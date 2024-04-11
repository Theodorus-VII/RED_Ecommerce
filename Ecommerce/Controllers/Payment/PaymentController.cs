using Ecommerce.Controllers.Payment.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Ecommerce.Util;
using Newtonsoft.Json;


namespace Ecommerce.Controllers.Payment
{
    [Authorize]
    [ApiController]
    [Route("payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("makePayment")]
        public async Task<IActionResult> MakePaymentAsync([FromBody] PaymentRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (request == null || request.Currency == null)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid request", null));
                    return BadRequest(errorResponse);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var firstName = User.FindFirstValue(ClaimTypes.GivenName);
                var lastName = User.FindFirstValue(ClaimTypes.Surname);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Unauthorized access.", null));
                    return Unauthorized(errorResponse);
                }
                var response = await _paymentService.MakePaymentAsync(userId, email, firstName, lastName, request.Currency, request.ReturnUrl, request.PhoneNumber);
                return Ok(new ApiResponse<object>(true, "successful payment request to chapa.", response));
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }
            
        }

        [HttpGet("verifypayment/{TxRef}")]
        public async Task<IActionResult> VerifyPaymentAsync(string TxRef , PaymentVerifyDTO request)
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
                if(request.Equals(null) || request.ShippingAddressId <= 0 || request.BillingAddressId <= 0)
                {
                    var errorResponse = JsonConvert.SerializeObject(new ApiResponse<object>(false, "Invalid request", null));
                    return BadRequest(errorResponse);
                }   
                var response = await _paymentService.VerifyTransactionAsync(userId, TxRef, request.ShippingAddressId, request.BillingAddressId);
                return Ok(new ApiResponse<object>(true, "successful payment request to chapa.", new { OrderNumber = response }));
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(new ApiResponse<object>(false, ex.Message, null));
                return BadRequest(response);
            }
            
        }
    }
}
