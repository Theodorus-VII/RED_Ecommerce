using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Add this namespace

namespace Ecommerce.Controllers.Cart
{
    [Authorize] // Assuming users need to be authenticated to access the cart
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
                return Unauthorized(); // Or handle authentication failure appropriately
            }

            var cartDTO = await _shoppingCartService.GetCartItemsAsync(userId);
            if (cartDTO.Items != null)
            {
                return Ok(cartDTO); // Return cart DTO if found
            }
            else
            {
                return NotFound("No cart found for this user."); // Or handle cart not found scenario
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

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
                await _shoppingCartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
                return Ok("Item added to cart successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return a bad request with the error message
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding the item to the cart."); // Return a server error status code with a generic error message
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
                    return Unauthorized(); // Or handle authentication failure appropriately
                }

                if (request.Items == null || request.Items.Count == 0)
                {
                    return BadRequest("Items list is empty or null."); // Return bad request if items list is empty or null
                }

                await _shoppingCartService.AddMultipleItemsToCartAsync(userId, request.Items);
                return Ok("Items added to cart successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return a bad request with the specific error message
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding multiple items to the cart."); // Return a server error status code with a generic error message
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

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
                await _shoppingCartService.UpdateCartItemQuantityAsync(userId, request.CartItemId, request.NewQuantity);
                return Ok("Item updated to cart successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return a bad request with the error message
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); // Return a server error status code with a generic error message
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
                    return Unauthorized(); // Or handle authentication failure appropriately
                }

                if (request.CartItemId <= 0)
                {
                    return BadRequest("Invalid CartItemId."); // Return bad request if CartItemsId is invalid
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
                return BadRequest(ex.Message); // Return a bad request with the error message
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); // Return a server error status code with a generic error message
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
                    return Unauthorized(); // Or handle authentication failure appropriately
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
                return BadRequest(ex.Message); // Return a bad request with the error message
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); // Return a server error status code with a generic error message
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
                    return Unauthorized(); // Or handle authentication failure appropriately
                }
                await _shoppingCartService.ClearCartAsync(userId);
                return Ok("Cart cleared successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return a bad request with the error message
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating an item in the cart."); // Return a server error status code with a generic error message
            }
        }
    }
}