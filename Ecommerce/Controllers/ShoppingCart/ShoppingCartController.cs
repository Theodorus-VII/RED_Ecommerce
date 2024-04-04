using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 

namespace Ecommerce.Controllers.Cart
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }

            var cartDTO = await _shoppingCartService.GetCartItemsAsync(userId);
            if (cartDTO.Items != null)
            {
                return Ok(cartDTO);
            }
            else
            {
                return NotFound("No cart found for this user."); 
            }
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                await _shoppingCartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
                return Ok("Item added to cart successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding the item to the cart."); 
            }
        }

        [HttpPost("add-multiple")]
        public async Task<IActionResult> AddMultipleItemsToCart([FromBody] AddMultipleItemsToCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(); 
                }

                if (request.Items == null || request.Items.Count == 0)
                {
                    return BadRequest("Items list is empty or null.");
                }

                await _shoppingCartService.AddMultipleItemsToCartAsync(userId, request.Items);
                return Ok("Items added to cart successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding multiple items to the cart.");
            }
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemQuantityRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                await _shoppingCartService.UpdateCartItemQuantityAsync(userId, request.CartItemId, request.NewQuantity);
                return Ok("Item updated to cart successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); 
            }
        }


        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get userId from token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(); 
                }

                if (request.CartItemId <= 0)
                {
                    return BadRequest("Invalid CartItemId."); 
                }

                await _shoppingCartService.RemoveFromCartAsync(userId, request.CartItemId);
                return Ok("Cart item removed successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); 
            }
        }





        [HttpDelete("remove-multiple")]
        public async Task<IActionResult> DeleteCartItems([FromBody] DeleteCartItemsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get userId from token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                if (request.CartItemIds != null)
                {
                    await _shoppingCartService.RemoveMultipleItemsFromCartAsync(userId, request.CartItemIds);
                    return Ok("Cart items removed successfully.");
                }
                else
                {
                    return BadRequest("Cart item IDs cannot be null.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); 
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(); 
                }
                await _shoppingCartService.ClearCartAsync(userId);
                return Ok("Cart cleared successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); 
            }
        }
    }
}