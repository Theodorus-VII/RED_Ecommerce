using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; // Update namespace to Ecommerce.Models
using Ecommerce.Services; // Update namespace to Ecommerce.Services

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("address")]
        public async Task<IActionResult> EnterAddress(Address address)
        {
            var result = await _checkoutService.EnterAddressAsync(address);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("payment")]
        public async Task<IActionResult> MakePayment(PaymentInfo paymentInfo)
        {
            var result = await _checkoutService.MakePaymentAsync(paymentInfo);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("order-history/{userId}")]
        public async Task<IActionResult> GetOrderHistory(string userId)
        {
            var orders = await _checkoutService.GetOrderHistoryAsync(userId);
            return Ok(orders);
        }
    }
}
