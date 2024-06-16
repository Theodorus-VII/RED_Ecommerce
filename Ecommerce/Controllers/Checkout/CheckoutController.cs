using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Ecommerce.Controllers.Checkout
{
    [Authorize]
    [ApiController]
    [Route("address")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        /// <summary>
        /// Get shipping addresses
        /// </summary>
        /// <returns>returns shipping addresses of a user</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /address/shipping
        /// </remarks>
        /// <response code="200">Returns shipping addresses of a user</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No shipping addresses found for the user</response>
        /// <response code="400">Bad request</response>
        [HttpGet("shipping")]
        public async Task<IActionResult> GetShippingAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var addresses = await _checkoutService.GetAddressesAsync(userId, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Successfully addresses fetched.", addresses));
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Get billing addresses
        /// </summary>
        /// <returns>returns billing addresses of a user</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /address/billing
        /// </remarks>
        /// <response code="200">Returns billing addresses of a user</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No billing addresses found for the user</response>
        /// <response code="400">Bad request</response>
        [HttpGet("billing")]
        public async Task<IActionResult> GetBillingAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var addresses = await _checkoutService.GetAddressesAsync(userId, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Successfully addresses fetched.", addresses));
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }


        /// <summary>
        /// Get a specific shipping address
        /// </summary>
        /// <param name="addressId">The addressId of the address to fetch</param>
        /// <returns>returns shipping address of a user with specified addressId</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /address/shipping/1
        /// </remarks>
        /// <response code="200">Returns shipping address of a user with specified addressId</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No shipping address found for the user with specified addressId</response>
        /// <response code="400">Bad request</response>
        [HttpGet("shipping/{addressId}")]
        public async Task<IActionResult> GetShippingAddressById(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var address = await _checkoutService.GetAddressByIdAsync(userId, addressId, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Successfully address fetched.", address));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Get a specific billing address
        /// </summary>
        /// <param name="addressId">The addressId of the address to fetch</param>
        /// <returns>returns billing address of a user with specified addressId</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /address/billing/1
        /// </remarks>
        /// <response code="200">Returns billing address of a user with the specified addressId</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No billing address found for the user with specified addressId</response>
        /// <response code="400">Bad request</response>
        [HttpGet("billing/{addressId}")]
        public async Task<IActionResult> GetBillingAddressById(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var address = await _checkoutService.GetAddressByIdAsync(userId, addressId, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Successfully address fetched.", address));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }


        /// <summary>
        /// Create a shipping address
        /// </summary>
        /// <param name="address">The shipping address to create</param>
        /// <returns>returns the shipping address created with in an apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /address/shipping
        ///     {
        ///         "Street": "123 Main St",
        ///         "City": "Example City",
        ///         "State": "Example State",
        ///         "Country": "Example Country",
        ///         "PostalCode": "12345"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the shipping address created with in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="400">Bad request</response>
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
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var result = await _checkoutService.AddAddressAsync(userId, address, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Successfully shipping address added.", result));
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Create a billing address
        /// </summary>
        /// <param name="address">The billing address to create</param>
        /// <returns>returns the billing address created with in an apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /address/billing
        ///     {
        ///         "Street": "123 Main St",
        ///         "City": "Example City",
        ///         "State": "Example State",
        ///         "Country": "Example Country",
        ///         "PostalCode": "12345"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the billing address created with in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="400">Bad request</response>
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
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var result = await _checkoutService.AddAddressAsync(userId, address, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Successfully billing address added.", result));

            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Update a shipping address
        /// </summary>
        /// <param name="address">The shipping address to update</param>
        /// <returns>returns the shipping address updated with in an apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /address/shipping/update
        ///     {
        ///         "AddressId": 1,
        ///         "Street": "123 Main St",
        ///         "City": "Example City",
        ///         "State": "Example State",
        ///         "Country": "Example Country",
        ///         "PostalCode": "12347"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the shipping address created with in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No shipping address found for the user with specified addressId in the address object</response>
        /// <response code="400">Bad request</response>
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
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var result = await _checkoutService.UpdateAddressAsync(userId, address, AddressType.Shipping);
                return Ok(new ApiResponse<object>(true, "Address updated successfully.", result));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }


        /// <summary>
        /// Update a billing address
        /// </summary>
        /// <param name="address">The billing address to update</param>
        /// <returns>returns the billing address updated with in an apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     
        ///     PUT /address/billing/update
        ///     {
        ///         "AddressId": 1,
        ///         "Street": "123 Main St",
        ///         "City": "Example City",
        ///         "State": "Example State",
        ///         "Country": "Example Country",
        ///         "PostalCode": "12347"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the billing address created with in an apiresponse object</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No billing address found for the user with specified addressId in the address object</response>
        /// <response code="400">Bad request</response>
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
                    var response = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(response);
                }
                var result = await _checkoutService.UpdateAddressAsync(userId, address, AddressType.Billing);
                return Ok(new ApiResponse<object>(true, "Address updated successfully.", result));
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }
        
        /// <summary>
        /// Remove a shipping address by addressId
        /// </summary>
        /// <param name="addressId">The addressId of the address to remove</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /address/shipping/remove/1
        /// </remarks>
        /// <response code="200">Returns an apiresponse object with success message</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No shipping address found for the user with specified addressId</response>
        /// <response code="400">Bad request</response>
        [HttpDelete("shipping/remove/{addressId}")]
        public async Task<IActionResult> RemoveAddress(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (addressId <= 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid CartItemId.", null);
                    return BadRequest(errorResponse);
                }
                await _checkoutService.RemoveAddressAsync(userId, addressId, AddressType.Shipping);
                var response = new ApiResponse<object>(true, "Address removed successfully.", null);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Remove a billing address by addressId
        /// </summary>
        /// <param name="addressId">The addressId of the address to remove</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /address/billing/remove/1
        /// </remarks>
        /// <response code="200">Returns an apiresponse object with success message</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No billing address found for the user with specified addressId</response>
        /// <response code="400">Bad request</response>
        [HttpDelete("billing/remove/{addressId}")]
        public async Task<IActionResult> RemoveBillingAddress(int addressId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (addressId <= 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid CartItemId.", null);
                    return BadRequest(errorResponse);
                }
                await _checkoutService.RemoveAddressAsync(userId, addressId, AddressType.Billing);
                var response = new ApiResponse<object>(true, "Address removed successfully.", null);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Remove shipping addresses which their id specified in the request body    
        /// </summary>
        /// <param name="request">List of addressIds of addresses to remove</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /address/shipping/removemultiple
        ///         
        ///         "addressIds": [1, 2, 3]
        ///         
        /// </remarks>
        /// <response code="200">Returns an apiresponse object with success message</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No shipping addresses found for the user with one of specified addressIds</response>
        /// <response code="400">Bad request</response>
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
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (request.AddressIds == null || request.AddressIds.Count == 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "AddressIds cannot be empty.", null);
                    return BadRequest(errorResponse);
                }

                await _checkoutService.RemoveMultipleAddresses(userId, request.AddressIds, AddressType.Shipping);
                var response = new ApiResponse<object>(true, "Addresses removed successfully.", null);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }


        /// <summary>
        /// Remove billing addresses which their id specified in the request body    
        /// </summary>
        /// <param name="request">List of addressIds of addresses to remove</param>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /address/billing/removemultiple
        ///         
        ///         "addressIds": [1, 2, 3]
        ///         
        /// </remarks>
        /// <response code="200">Returns an apiresponse object with success message</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No billing addresses found for the user with one of specified addressIds</response>
        /// <response code="400">Bad request</response>
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
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if (request.AddressIds == null || request.AddressIds.Count == 0)
                {
                    var errorResponse = new ApiResponse<object>(false, "AddressIds cannot be empty.", null);
                    return BadRequest(errorResponse);
                }

                await _checkoutService.RemoveMultipleAddresses(userId, request.AddressIds, AddressType.Billing);
                var response = new ApiResponse<object>(true, "Addresses removed successfully.", null);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return Unauthorized(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Clear all shipping addresses of a user
        /// </summary>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /address/shipping/clear
        /// </remarks>
        /// <response code="200">Returns an apiresponse object with success message</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No shipping addresses found for the user</response>
        /// <response code="400">Bad request</response>
        [HttpDelete("shipping/clear")]
        public async Task<IActionResult> ClearAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                await _checkoutService.ClearAddressesAsync(userId, AddressType.Shipping);
                var response = new ApiResponse<object>(true, "Addresses cleared successfully.", null);
                return Ok(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Clear all billing addresses of a user
        /// </summary>
        /// <returns>returns an apiresponse object with success message</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /address/billing/clear
        /// </remarks>
        /// <response code="200">Returns an apiresponse object with success message</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">No billing addresses found for the user</response>
        /// <response code="400">Bad request</response>
        [HttpDelete("billing/clear")]
        public async Task<IActionResult> ClearBillingAddresses()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                await _checkoutService.ClearAddressesAsync(userId, AddressType.Billing);
                var response = new ApiResponse<object>(true, "Addresses cleared successfully.", null);
                return Ok(response);
            }
            catch(ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }

        }

        
    }
}
