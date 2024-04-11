using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Newtonsoft.Json;
using Ecommerce.Models;

namespace Ecommerce.Controllers.Checkout
{
    [Authorize]
    [ApiController]
    [Route("address")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }


        [HttpGet("shipping")]
        public async Task<IActionResult> GetShippingAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var addresses = await _checkoutService.GetAddressesAsync(userId, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Successfully addresses fetched.", addresses));
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }


        [HttpGet("billing")]
        public async Task<IActionResult> GetBillingAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var addresses = await _checkoutService.GetAddressesAsync(userId, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Successfully addresses fetched.", addresses));
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }



        [HttpGet("shipping/{addressId}")]
        public async Task<IActionResult> GetShippingAddressById(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var address = await _checkoutService.GetAddressByIdAsync(userId, addressId, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Successfully address fetched.", address));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpGet("billing/{addressId}")]
        public async Task<IActionResult> GetBillingAddressById(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var address = await _checkoutService.GetAddressByIdAsync(userId, addressId, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Successfully address fetched.", address));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpPost("shipping")]
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
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var result = await _checkoutService.AddAddressAsync(userId, address, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Successfully shipping address added.", result));
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpPost("billing")]
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
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var result = await _checkoutService.AddAddressAsync(userId, address, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Successfully billing address added.", result));

            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpPut("shipping/update")]
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
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var result = await _checkoutService.UpdateAddressAsync(userId, address, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Address updated successfully.", result));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpPut("billing/update")]
        public async Task<IActionResult> UpdateBillingAddress(UpdateAddressRequestDTO address)
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
                    var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(response);
                }
                var result = await _checkoutService.UpdateAddressAsync(userId, address, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Address updated successfully.", result));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpDelete("shipping/remove/{addressId}")]
        public async Task<IActionResult> RemoveAddress(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (addressId <= 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid CartItemId.", null));
                    return BadRequest(errorResponse);
                }
                await _checkoutService.RemoveAddressAsync(userId, addressId, AddressType.Shipping);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Address removed successfully.", null));
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpDelete("billing/remove/{addressId}")]
        public async Task<IActionResult> RemoveBillingAddress(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (addressId <= 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid CartItemId.", null));
                    return BadRequest(errorResponse);
                }
                await _checkoutService.RemoveAddressAsync(userId, addressId, AddressType.Billing);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Address removed successfully.", null));
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpDelete("shipping/removemultiple")]
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
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                if (request.AddressIds == null || request.AddressIds.Count == 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "AddressIds cannot be empty.", null));
                    return BadRequest(errorResponse);
                }

                await _checkoutService.RemoveMultipleAddresses(userId, request.AddressIds, AddressType.Shipping);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Addresses removed successfully.", null));
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpDelete("billing/removemultiple")]
        public async Task<IActionResult> RemoveMultipleBillingAddresses(RemoveMultipleAddressRequestDTO request)
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
                if (request.AddressIds == null || request.AddressIds.Count == 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "AddressIds cannot be empty.", null));
                    return BadRequest(errorResponse);
                }

                await _checkoutService.RemoveMultipleAddresses(userId, request.AddressIds, AddressType.Billing);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Addresses removed successfully.", null));
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpDelete("shipping/clear")]
        public async Task<IActionResult> ClearAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                await _checkoutService.ClearAddressesAsync(userId, AddressType.Shipping);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Addresses cleared successfully.", null));
                return Ok(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        [HttpDelete("billing/clear")]
        public async Task<IActionResult> ClearBillingAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                await _checkoutService.ClearAddressesAsync(userId, AddressType.Billing);
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(true, "Addresses cleared successfully.", null));
                return Ok(response);
            }
            catch(ArgumentException ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }

        }

        
    }
}
