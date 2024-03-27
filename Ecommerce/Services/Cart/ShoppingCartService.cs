using AutoMapper;
using Ecommerce.Controllers.Cart.Contracts;
using Ecommerce.Data;
using Microsoft.EntityFrameworkCore; // Assuming Entity Framework Core is used for database access

public class ShoppingCartService : IShoppingCartService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper; // Add AutoMapper IMapper

    public ShoppingCartService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        // Check if the product already exists in the user's cart
        var existingCartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (existingCartItem != null)
        {
            // Update the quantity if the product is already in the cart
            existingCartItem.Quantity += quantity;
        }
        else
        {
            // Add a new item to the cart if it doesn't exist
            var newCartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };
            _context.CartItems.Add(newCartItem);
        }

        await _context.SaveChangesAsync();
    }
    public async Task AddMultipleItemsToCartAsync(string userId, List<AddToCartRequest> items)
    {
        foreach (var item in items)
        {
            await AddToCartAsync(userId, item.ProductId, item.Quantity);
        }
    }

    public async Task RemoveFromCartAsync(string userId, int productId)
    {
        var cartItemToRemove = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItemToRemove != null)
        {
            _context.CartItems.Remove(cartItemToRemove);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateCartItemQuantityAsync(string userId, int productId, int newQuantity)
    {
        var cartItemToUpdate = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItemToUpdate != null)
        {
            cartItemToUpdate.Quantity = newQuantity;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<CartItemResponseDTO>> GetCartItemsAsync(string userId)
    {
        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product) // Include product details if needed
            .ToListAsync();

        return _mapper.Map<List<CartItemResponseDTO>>(cartItems);
    }

    public async Task DeleteCartItemsAsync(string userId, List<int> cartItemIds)
    {
        var cartItemsToDelete = await _context.CartItems
            .Where(c => c.UserId == userId && cartItemIds.Contains(c.CartItemId))
            .ToListAsync();

        _context.CartItems.RemoveRange(cartItemsToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task ClearCartAsync(string userId)
    {
        var cartItemsToDelete = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        _context.CartItems.RemoveRange(cartItemsToDelete);
        await _context.SaveChangesAsync();
    }
}
