using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models; // Update namespace to Ecommerce.Models
using Ecommerce.Services.Checkout;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Controllers.Checkout.Contracts;

namespace Ecommerce.Controllers.Checkout
{
    [Authorize]
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("shippingaddress")]
        public async Task<IActionResult> EnterShippingAddress(AddressRequestDTO address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _checkoutService.EnterShippingAddressAsync(userId, address);
            return Ok(result);
        }

        [HttpPost("billingaddress")]
        public async Task<IActionResult> EnterBillingAddress(AddressRequestDTO address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _checkoutService.EnterBillingAddressAsync(userId, address);
            return Ok(result);

        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = await _checkoutService.GetAddressesAsync(userId);
            return Ok(addresses);
        }
    }
}
