using Ecommerce.Controllers.Orders.Contracts;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers.Orders
{
  
    [ApiController]
    [Route("order")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        /// <summary>
        /// Get all orders of a user
        /// </summary>
        /// <returns>returns list of orders in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /order
        /// </remarks>
        /// <response code="200">Returns list of orders with in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No orders found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                var orders = await _orderService.GetOrdersAsync(userId);
                if (orders.Count == 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "No orders found.", null);
                    return NotFound(errorResponse);
                }
                var response = new ApiResponse<List<OrderResponseDTO>>(true, "Orders fetched successfully.", orders);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Get order by order id
        /// </summary>
        /// <param name="orderId">The orderId of the order to fetch</param>
        /// <returns>returns an order in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /order/1
        /// </remarks>
        /// <response code="200">Returns an order with in an apiresponse object</response>
        /// <response code="400">Invalid order id</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Order not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderAsync(int orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (orderId <= 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid order id.", null);
                    return BadRequest(errorResponse);
                }
                var order = await _orderService.GetOrderAsync(userId, orderId);
                if (order == null)
                {
                    var errorResponse = new ApiResponse<object>(false, "Order not found.", null);
                    return NotFound(errorResponse);
                }
                var response = new ApiResponse<OrderResponseDTO>(true, "Order fetched successfully.", order);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Get order by order number
        /// </summary>
        /// <param name="orderNumber">The orderNumber of the order to fetch</param>
        /// <returns>returns an order in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /order/number/b66f4086-74f2-46ba-a84e-b2f588c95ed1
        /// </remarks>
        [HttpGet("number/{orderNumber}")]
        public async Task<IActionResult> GetOrderByOrderNumberAsync(string orderNumber)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(orderNumber))
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid order number.", null);
                    return BadRequest(errorResponse);
                }
                var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
                if (order == null)
                {
                    var errorResponse = new ApiResponse<object>(false, "Order not found.", null);
                    return NotFound(errorResponse);
                }
                var response = new ApiResponse<OrderResponseDTO>(true, "Order fetched successfully.", order);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId">The orderId of the order to update</param>
        /// <param name="status">The status of the order to update</param>
        /// <returns>returns success message in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     PATCH /order/status/1
        ///         "Delivered"
        /// </remarks>
        /// <response code="200">Returns success message in apiresponse object</response>
        /// <response code="400">Invalid order id or status</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Order not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("status/{orderId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateOrderStatusAsync(int orderId, [FromBody] string status)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = User.FindFirstValue(ClaimTypes.Role);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (orderId <= 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid order id.", null);
                    return BadRequest(errorResponse);
                }
                if (string.IsNullOrEmpty(status))
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid status.", null);
                    return BadRequest(errorResponse);
                }
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                var response = new ApiResponse<object>(true, "Order status updated successfully.", null);
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
    }
}
