using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 

namespace Ecommerce.Controllers.Cart
{ 
    [ApiController]
    [Route("cart")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        /// <summary>
        /// Get all items in the cart
        /// </summary>
        /// <returns>returns a cart that contains list of cart items in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /cart
        /// </remarks>
        /// <response code="200">Returns the cart items in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response> 
        /// <response code="404">No items found in the cart</response>
        /// <response code="404">Cart not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet()]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }

                var cartDTO = await _shoppingCartService.GetCartItemsAsync(userId);
                var response = new ApiResponse<object>(true, "Cart items fetched successfully.", cartDTO);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, errorResponse);
            }
            
        }


        /// <summary>
        /// Get cart item by id
        /// </summary>
        /// <param name="cartItemId">Id of the cart item to fetch</param>
        /// <returns>returns a cart item in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /cart/1
        /// </remarks>
        /// <response code="200">Returns the cart item in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="404">Cart not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{cartItemId}")]
        public async Task<IActionResult> GetCartItemById(int cartItemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }

                var cartItemDTO = await _shoppingCartService.GetCartItemsById(userId, cartItemId);
                var response = new ApiResponse<object>(true, "Cart item fetched successfully.", cartItemDTO);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, errorResponse);
            }
        }


        /// <summary>
        /// Add item to cart
        /// </summary>
        /// <param name="request">request object that contains productid and quantity</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /cart/add
        ///         {
        ///             "productId": 1,
        ///             "quantity": 2
        ///         }
        /// </remarks>
        /// <response code="200">Returns success message in an apiresponse object</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
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
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                
                await _shoppingCartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
                var response = new ApiResponse<object>(true, "Item added to cart successfully.", null);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = new ApiResponse<object>(false, "An error occurred while adding the item to the cart.", null);
                return StatusCode(500, errorResponse); 
            }
        }

        /// <summary>
        /// Add items to cart
        /// </summary>
        /// <param name="request">request object that contains list of items</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /cart/add
        ///         {
        ///             "items": [
        ///                 {
        ///                     "productId": 1,
        ///                     "quantity": 2
        ///                 },
        ///                 {
        ///                     "productId": 2,
        ///                     "quantity": 4
        ///                 },
        ///                 {
        ///                     "productId": 3,
        ///                     "quantity": 2
        ///                 },
        ///                 {
        ///                     "productId": 4,
        ///                     "quantity": 2
        ///                 }
        ///             ]
        ///         }
        /// </remarks>
        /// <response code="200">Returns success message in an apiresponse object</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
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
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse); 
                }

                if (request.Items == null || request.Items.Count == 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "Items list is empty or null.", null);
                    return BadRequest(errorResponse);
                }

                await _shoppingCartService.AddMultipleItemsToCartAsync(userId, request.Items);
                var response = new ApiResponse<object>(true, "Items added to cart successfully.", null);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = new ApiResponse<object>(false, "An error occurred while adding multiple items to the cart.", null);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        /// <param name="request">request that contains the cartItemId and the newQuantity</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     PUT /cart/update
        ///         {
        ///             "cartItemId": 1,
        ///             "newQuantity": 5
        ///         }
        /// </remarks>
        /// <response code="200">Returns success message in an apiresponse object</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="404">Cart not found</response>
        /// <response code="500">Internal server error</response>
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
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                await _shoppingCartService.UpdateCartItemQuantityAsync(userId, request.CartItemId, request.NewQuantity);
                var response = new ApiResponse<object>(true, "Item updated successfully.", null);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);   
                return Unauthorized(errorResponse);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "Cart item not found" || ex.Message == "Cart not found")
                {
                    var response = new ApiResponse<object>(false, ex.Message, null);
                    return NotFound(response); 
                }
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null);
                return StatusCode(500, errorResponse); 
            }
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        /// <param name="cartItemId">Id of the cartItem to be removed</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /cart/remove/1
        /// </remarks>
        /// <response code="200">Returns success message in an apiresponse object</response>
        /// <response code="400">Invalid CartItemId</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="404">Cart not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse); 
                }

                if (cartItemId <= 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid CartItemId.", null);
                    return BadRequest(errorResponse); 
                }

                await _shoppingCartService.RemoveFromCartAsync(userId, cartItemId);
                var response = new ApiResponse<object>(true, "Cart item removed successfully.", null);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(errorResponse);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null);
                return StatusCode(500, errorResponse); 
            }
        }




        /// <summary>
        /// Remove items from cart
        /// </summary>
        /// <param name="request">Ids of the cartItems to be removed</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /cart/removemultiple
        ///         {
        ///             "cartItemIds": [
        ///                 1,2,3
        ///             ]
        ///         }
        /// </remarks>
        /// <response code="200">Returns success message in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Cart item not found</response>
        /// <response code="404">Cart not found</response>
        /// <response code="400">Invalid CartItemIds</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("removemultiple")]
        public async Task<IActionResult> DeleteCartItems([FromBody] DeleteCartItemsRequest request)
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
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (request.CartItemIds != null)
                {
                    await _shoppingCartService.RemoveMultipleItemsFromCartAsync(userId, request.CartItemIds);
                    var response = new ApiResponse<object>(true, "Cart items removed successfully.", null);
                    return Ok(response);
                }
                else
                {
                    var errorResponse = new ApiResponse<object>(false, "Cart item IDs cannot be null.", null);
                    return BadRequest(errorResponse);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(errorResponse);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null);
                return StatusCode(500, errorResponse); 
            }
        }


        /// <summary>
        /// Clear cart
        /// </summary>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /cart/clear
        /// </remarks>
        /// <response code="200">Returns success message in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Cart not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse); 
                }
                await _shoppingCartService.ClearCartAsync(userId);
                var response = new ApiResponse<object>(true, "Cart cleared successfully.", null);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(errorResponse); 
            }
            catch (Exception)
            {
                var errorResponse = new ApiResponse<object>(false, "An error occurred while updating an item in the cart.", null);
                return StatusCode(500, errorResponse); 
            }
        }
    }
}