using Ecommerce.Controllers.Payment.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.Services.Interfaces;
using Newtonsoft.Json;
using ChapaNET;
using static ChapaNET.Chapa;

namespace Ecommerce.Controllers.Payment
{
    [Authorize]
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly HttpClient _httpClient;

        public PaymentController(IPaymentService paymentService, HttpClient httpClient)
        {
            _paymentService = paymentService;
            _httpClient = httpClient;
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
                if (request == null)
                {
                    return BadRequest("Invalid request");
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var firstName = User.FindFirstValue(ClaimTypes.GivenName);
                var lastName = User.FindFirstValue(ClaimTypes.Surname);
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    return Unauthorized();
                }

                var response = await _paymentService.MakePaymentAsync(request, email, firstName, lastName);
                return Ok(new { responsedto = response });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("verifypayment")]
        public async Task<IActionResult> VerifyPaymentAsync([FromBody] PaymentVerifyDTO request)
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
                if (request.TxRef == null)
                {
                    return BadRequest("Invalid request");
                }
                //var apiUrl = "https://api.chapa.co/v1/transaction/verify/" + request.TxRef;
                //var requesttochapa = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                //requesttochapa.Headers.Add("Authorization", "Bearer CHASECK_TEST-vQSKrHiFWiiwQaqJmxb0pEbSQrVsHZgT ");
                //var response = await _httpClient.SendAsync(requesttochapa);
                //if (!response.IsSuccessStatusCode)
                //{
                //    return BadRequest("An error occurred while verifying payment");
                //}
                //var content = await response.Content.ReadAsStringAsync();
                //PaymentResponse paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(content);

                //return Ok(paymentResponse);




                await _paymentService.VerifyTransactionAsync(userId, request.TxRef);
                return Ok("payment successfull.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
