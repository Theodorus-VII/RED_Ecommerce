using Ecommerce.Controllers.Orders.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Ecommerce.Controllers.Orders
{
    [Authorize]
    [ApiController]
    [Route("order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                var orders = await _orderService.GetOrdersAsync(userId);
                if (orders.Count == 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "No orders found.", null));
                    return NotFound(errorResponse);
                }
                var response = new ApiResponse<List<OrderResponseDTO>>(true, "Orders fetched successfully.", orders);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderAsync(int orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (orderId <= 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid order id.", null));
                    return BadRequest(errorResponse);
                }
                var order = await _orderService.GetOrderAsync(userId, orderId);
                if (order == null)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Order not found.", null));
                    return NotFound(errorResponse);
                }
                var response = new ApiResponse<OrderItemResponseDTO>(true, "Order fetched successfully.", order);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return StatusCode(500, errorResponse);
            }
        }


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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid order number.", null));
                    return BadRequest(errorResponse);
                }
                var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
                if (order == null)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Order not found.", null));
                    return NotFound(errorResponse);
                }
                var response = new ApiResponse<OrderItemResponseDTO>(true, "Order fetched successfully.", order);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPatch("status/{orderId}")]
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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (role != "Admin")
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (orderId <= 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid order id.", null));
                    return BadRequest(errorResponse);
                }
                if (string.IsNullOrEmpty(status))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid status.", null));
                    return BadRequest(errorResponse);
                }
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                var response = new ApiResponse<object>(true, "Order status updated successfully.", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return StatusCode(500, errorResponse);
            }
        }
    }
}
