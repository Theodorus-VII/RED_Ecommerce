using AutoMapper;
using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<IShoppingCartService> _logger;

        public ShoppingCartService(ApplicationDbContext context, IMapper mapper, ILogger<IShoppingCartService> logger)
        {

            
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<CartResponseDTO> GetCartItemsAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product).ThenInclude(p => p.Images)
                .FirstOrDefaultAsync() ?? throw new ArgumentException("Cart not found.");
                var cartDTO = _mapper.Map<CartResponseDTO>(cart);
                return cartDTO;
                
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception: {}", e);
                throw new Exception("An error occurred while fetching cart items.");
            }
        }


        public async Task<CartItemResponseDTO> GetCartItemsById(string userId, int cartItemId)
        {
            try
            {
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId) ?? throw new ArgumentException("Cart not found.");
                Console.WriteLine(cart.CartId);
                var cartItem = await _context.CartItems.Where(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId).Include(ci => ci.Product).ThenInclude(p => p.Images).FirstOrDefaultAsync() ?? throw new ArgumentException("Cart item not found.");
                return _mapper.Map<CartItemResponseDTO>(cartItem); 
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while fetching cart item.");
            }
        }


        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId) ?? throw new ArgumentException("Invalid product ID.");
                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }
                var existingCart = await _context.Carts.Include(c => c.Items)
                                                       .FirstOrDefaultAsync(c => c.UserId == userId);

                if (existingCart != null)
                {
                    existingCart.UpdatedAt = DateTime.Now;
                    var existingCartItem = existingCart?.Items?.FirstOrDefault(ci => ci.ProductId == productId);

                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += quantity;
                        existingCartItem.Price += quantity * product.Price;
                        existingCartItem.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        existingCart?.Items?.Add(new CartItem
                        {
                            CartId = existingCart.CartId,
                            ProductId = productId,
                            Quantity = quantity,
                            Price = quantity * product.Price
                        });
                    }
                    existingCart?.UpdateTotalPrice();
                }
                else
                {
                    var newCart = new Cart
                    {
                        UserId = userId,
                        Items = new List<CartItem>
                            { new() {
                                ProductId = productId,
                                Quantity = quantity,
                                Price = quantity * product.Price
                            } },
                    };
                    newCart.UpdateTotalPrice();
                    _context.Carts.Add(newCart);
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task AddMultipleItemsToCartAsync(string userId, List<AddToCartRequest> items)
        {
            try
            {
                foreach (var item in items)
                {
                    await AddToCartAsync(userId, item.ProductId, item.Quantity);
                }
            }
            catch
            {
                throw;
            }
        }




        public async Task UpdateCartItemQuantityAsync(string userId, int cartItemId, int newQuantity)
        {
            try
            {
                if (newQuantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }
                var cart = await _context.Carts
                    .Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null && cart.Items != null)
                {
                    var cartItemToUpdate = await _context.CartItems.FindAsync(cartItemId);
                    if (cartItemToUpdate != null)
                    {
                        if (cart.Items.Contains(cartItemToUpdate))
                        {
                            var product = await _context.Products.FindAsync(cartItemToUpdate.ProductId) ?? throw new ArgumentException("Invalid product ID.");
                            cartItemToUpdate.Quantity = newQuantity;
                            cartItemToUpdate.Price = newQuantity * product?.Price ?? 0;
                            cartItemToUpdate.UpdatedAt = DateTime.Now;
                            cart.UpdatedAt = DateTime.Now;
                            cart?.UpdateTotalPrice();
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            throw new UnauthorizedAccessException();
                        }

                    }
                    else
                    {
                        throw new ArgumentException("Cart item not found.");
                    }
                }
                else
                {
                    throw new ArgumentException("Cart not found.");
                }
            }
            catch 
            {
                throw;
            }
        }


        public async Task RemoveFromCartAsync(string userId, int cartItemId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);


                if (cart != null && cart.Items != null)
                {
                    var cartItemToRemove = await _context.CartItems.FindAsync(cartItemId);

                    if (cartItemToRemove != null)
                    {
                        if (cart.Items.Contains(cartItemToRemove))
                        {
                            cart.Items.Remove(cartItemToRemove);
                            cart.UpdateTotalPrice();
                            _context.CartItems.Remove(cartItemToRemove);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Cart item not found.");
                    }
                    if (cart.Items.Count == 0)
                    {
                        _context.Carts.Remove(cart);
                        await _context.SaveChangesAsync();
                    }

                }
                else
                {
                    throw new ArgumentException("Cart not found.");
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveMultipleItemsFromCartAsync(string userId, List<int> cartItemIds)
        {
            try
            {
                foreach (var id in cartItemIds)
                {
                    await RemoveFromCartAsync(userId, id);
                }
            }
            catch
            {
                throw;
            }
        }


        public async Task ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts.Where(c => c.UserId == userId).FirstOrDefaultAsync();
                if (cart != null)
                {
                    var cartItemsToDelete = await _context.CartItems
                    .Where(c => c.CartId == cart.CartId)
                    .ToListAsync();
                    if (cartItemsToDelete != null)
                    {
                        _context.CartItems.RemoveRange(cartItemsToDelete);
                        _context.Carts.Remove(cart);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new ArgumentException("Cart not found.");
                }
                

            }
            catch
            {
                throw;
            }
        }
    }
}