using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims; 

namespace Ecommerce.Controllers.Cart
{
    [Authorize] 
    [ApiController]
    [Route("cart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }

                var cartDTO = await _shoppingCartService.GetCartItemsAsync(userId);
                var response = new ApiResponse<object>(true, "Cart items fetched successfully.", cartDTO);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return StatusCode(500, errorResponse);
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
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                await _shoppingCartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Item added to cart successfully.", null));
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "An error occurred while adding the item to the cart.", null));
                return StatusCode(500, errorResponse); 
            }
        }

        [HttpPost("addmultiple")]
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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse); 
                }

                if (request.Items == null || request.Items.Count == 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Items list is empty or null.", null));
                    return BadRequest(errorResponse);
                }

                await _shoppingCartService.AddMultipleItemsToCartAsync(userId, request.Items);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Items added to cart successfully.", null));
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "An error occurred while adding multiple items to the cart.", null));
                return StatusCode(500, errorResponse);
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
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                await _shoppingCartService.UpdateCartItemQuantityAsync(userId, request.CartItemId, request.NewQuantity);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Item updated successfully.", null));
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));   
                return Unauthorized(errorResponse);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null));
                return StatusCode(500, errorResponse); 
            }
        }


        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse); 
                }

                if (cartItemId <= 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid CartItemId.", null));
                    return BadRequest(errorResponse); 
                }

                await _shoppingCartService.RemoveFromCartAsync(userId, cartItemId);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Cart item removed successfully.", null));
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(errorResponse);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null));
                return StatusCode(500, errorResponse); 
            }
        }





        [HttpDelete("removemultiple")]
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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (request.CartItemIds != null)
                {
                    await _shoppingCartService.RemoveMultipleItemsFromCartAsync(userId, request.CartItemIds);
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Cart items removed successfully.", null));
                    return Ok(response);
                }
                else
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Cart item IDs cannot be null.", null));
                    return BadRequest(errorResponse);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(errorResponse);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null));
                return StatusCode(500, errorResponse); 
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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse); 
                }
                await _shoppingCartService.ClearCartAsync(userId);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Cart cleared successfully.", null));
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null));
                return StatusCode(500, errorResponse); 
            }
        }
    }
}