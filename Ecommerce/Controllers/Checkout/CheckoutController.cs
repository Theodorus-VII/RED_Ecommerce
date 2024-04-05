using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Services.Interfaces;

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


        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                var addresses = await _checkoutService.GetAddressesAsync(userId);
                return Ok(addresses);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("address/{addressId}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                var address = await _checkoutService.GetAddressByIdAsync(userId, addressId);
                return Ok(address);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(ArgumentException)
            {
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("shippingaddress")]
        public async Task<IActionResult> EnterShippingAddress(AddressRequestDTO address)
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
                var result = await _checkoutService.AddAddressAsync(userId, address, "shipping");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("billingaddress")]
        public async Task<IActionResult> EnterBillingAddress(AddressRequestDTO address)
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
                var result = await _checkoutService.AddAddressAsync(userId, address, "billing");
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("updateaddress")]
        public async Task<IActionResult> UpdateAddress(UpdateAddressRequestDTO address)
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
                var result = await _checkoutService.UpdateAddressAsync(userId, address);
                return Ok(result);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(ArgumentException)
            {
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("removeaddress/{addressId}")]
        public async Task<IActionResult> RemoveAddress(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                if (addressId <= 0)
                {
                    return BadRequest("Invalid CartItemId.");
                }
                await _checkoutService.RemoveAddressAsync(userId, addressId);
                return Ok("Address removed successfully.");
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(ArgumentException)
            {
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("remove-multiple-addresses")]
        public async Task<IActionResult> RemoveMultipleAddresses(RemoveMultipleAddressRequestDTO request)
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
                if (request.AddressIds == null || request.AddressIds.Count == 0)
                {
                    return BadRequest("AddressIds cannot be empty.");
                }

                await _checkoutService.RemoveMultipleAddresses(userId, request.AddressIds);
                return Ok("Addresses removed successfully.");
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(ArgumentException)
            {
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("clearaddresses")]
        public async Task<IActionResult> ClearAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                await _checkoutService.ClearAddressesAsync(userId);
                return Ok("Addresses cleared successfully.");
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        
    }
}
