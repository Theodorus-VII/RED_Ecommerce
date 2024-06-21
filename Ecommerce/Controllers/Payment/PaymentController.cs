using Ecommerce.Controllers.Payment.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace Ecommerce.Controllers.Payment
{
    [ApiController]
    [Route("payment")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Make payment request to chapa for single product
        /// </summary>
        /// <param name="request">the request object used to make a payment request to chapa</param>
        /// <returns>returns the txref and checoutUrl in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /payment/singleproduct
        ///         request body:
        ///             {
        ///                 "currency": "ETB",
        ///                 "returnUrl": "https://example.com",
        ///                 "phoneNumber": "0912345678",
        ///                 "productId": 1,
        ///                 "quantity": 2 
        ///             }
        /// </remarks>
        /// <response code="200">Returns the txref and checoutUrl in an apiresponse object</response>
        /// <response code="400">Invalid request</response>
        /// <response code="400">Product out of stock</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("singleproduct")]

        public async Task<IActionResult> MakePaymentForSingleProduct([FromBody] SingleProductPaymentRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (request == null || request.Currency == null || request.ReturnUrl == null || request.ProductId < 1 || request.Quantity < 1)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid request", null);
                    return BadRequest(errorResponse);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var firstName = User.FindFirstValue(ClaimTypes.GivenName);
                var lastName = User.FindFirstValue(ClaimTypes.Surname);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                var response = await _paymentService.MakePaymentAsync(userId, email, firstName, lastName, request.Currency, request.ReturnUrl, request.PhoneNumber, request.ProductId, request.Quantity);
                return Ok(new ApiResponse<object>(true, "successful payment request to chapa.", response));
            }
            catch (ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, response);
            }

        }

        /// <summary>
        /// Make payment request to chapa for multiple products
        /// </summary>
        /// <param name="request">the request object used to make a payment request to chapa</param>
        /// <returns>returns the txref and checoutUrl in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /payment/makePayment
        ///         request body:
        ///             {
        ///                 "currency": "ETB",
        ///                 "returnUrl": "https://example.com",
        ///                 "phoneNumber": "0912345678"
        ///             }
        /// </remarks>
        /// <response code="200">Returns the txref and checoutUrl in an apiresponse object</response>
        /// <response code="400">Invalid request</response>
        /// <response code="400">Product out of stock</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("makePayment")]
        public async Task<IActionResult> MakePaymentAsync([FromBody] PaymentRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (request == null || request.Currency == null || request.ReturnUrl == null)
                {
                    var errorResponse = new ApiResponse<object>(false, "Invalid request", null);
                    return BadRequest(errorResponse);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var firstName = User.FindFirstValue(ClaimTypes.GivenName);
                var lastName = User.FindFirstValue(ClaimTypes.Surname);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                var response = await _paymentService.MakePaymentAsync(userId, email, firstName, lastName, request.Currency, request.ReturnUrl, request.PhoneNumber, null, null);
                return Ok(new ApiResponse<object>(true, "successful payment request to chapa.", response));
            }
            catch (ArgumentException ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(500, response);
            }

        }

        /// <summary>
        /// Verify payment transaction
        /// </summary>
        /// <param name="request">the request object used to verify a payment transaction and create Order</param>
        /// <returns>returns the order number in apiresponse object</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /payment/verifypayment
        ///         request body:
        ///             {
        ///                 "txRef": "tx638485309250761188",
        ///                 "shippingAddressId": 1,
        ///                 "billingAddressId": 1
        ///             }
        /// </remarks>
        /// <response code="200">Returns the order number in an apiresponse object</response>
        /// <response code="400">Invalid request</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized access</response>
        [HttpPost("verifypayment")]
        public async Task<IActionResult> VerifyPaymentAsync(PaymentVerifyDTO request)
        {
            try
            {
                _logger.LogInformation("Verifying payment...");
                if (!ModelState.IsValid)
                {
                    _logger.LogInformation("Invalid payment request: {}", ModelState);
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = new ApiResponse<object>(false, "Unauthorized access.", null);
                    return Unauthorized(errorResponse);
                }
                if(request.Equals(null) || request.ShippingAddressId <= 0 || request.BillingAddressId <= 0)
                {
                    _logger.LogInformation("Invalid payment request: sth with the billing and shipping addresses");
                    var errorResponse = new ApiResponse<object>(false, "Invalid request", null);
                    return BadRequest(errorResponse);
                }   
                if (string.IsNullOrEmpty(request.TxRef))
                {
                    _logger.LogInformation("Invalid txref");
                    var errorResponse = new ApiResponse<object>(false, "Invalid transaction reference.", null);
                    return BadRequest(errorResponse);
                }
                var response = await _paymentService.VerifyTransactionAsync(userId, request.TxRef, request.ShippingAddressId, request.BillingAddressId);
                return Ok(new ApiResponse<object>(true, "successful payment request to chapa.", new { OrderNumber = response }));
            }
            catch (Exception ex)
            {
                _logger.LogError("ServerError: {}", ex);
                var response = new ApiResponse<object>(false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            
        }
    }
}
