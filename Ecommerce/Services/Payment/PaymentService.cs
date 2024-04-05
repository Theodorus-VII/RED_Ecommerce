using ChapaNET;
using Ecommerce.Controllers.Payment.Contracts;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http;


namespace Ecommerce.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly Chapa _chapa;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient = new HttpClient();

        public PaymentService(ApplicationDbContext context)
        {
            _chapa = new Chapa("CHASECK_TEST-vQSKrHiFWiiwQaqJmxb0pEbSQrVsHZgT");
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task<Object> MakePaymentAsync(PaymentRequestDTO paymentRequest, string email, string firstName, string lastName)
        {
            try
            {
                if (paymentRequest.Currency == null)
                {
                    throw new ArgumentException("Currency is required");
                }
                Console.WriteLine("Payment request started. ");
                var txRef = Chapa.GetUniqueRef();

                var chapaRequest = new ChapaRequest(amount: paymentRequest.Amount, email: email, firstName: firstName,lastName: lastName,tx_ref: txRef,return_url: paymentRequest.ReturnUrl,
                    callback_url: "https://localhost:7195/api/payment/verifypayment", customTitle: paymentRequest.Customization.ItemType , customDescription: paymentRequest.Customization.Id);

                var result = await _chapa.RequestAsync(chapaRequest);
                if (result.Status != "success")
                {
                    throw new Exception("the request is not success.");
                }
                if (result.CheckoutUrl == null)
                {
                    throw new Exception("checkout url is null.");
                }
                Console.WriteLine("Payment request have finished. ");
                return new { TxRwf = txRef, Url = result.CheckoutUrl };
            }
            catch (Exception)
            {
                throw;
            }   
            
            
        }


        public async Task VerifyTransactionAsync(string userId, string transactionId)
        {
            try
            {
                var apiUrl = "https://api.chapa.co/v1/transaction/verify/" + transactionId;
                var requesttochapa = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                requesttochapa.Headers.Add("Authorization", "Bearer CHASECK_TEST-vQSKrHiFWiiwQaqJmxb0pEbSQrVsHZgT ");
                var response = await _httpClient.SendAsync(requesttochapa);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("An error occurred while verifying payment");
                }
                var content = await response.Content.ReadAsStringAsync();
                PaymentResponse paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(content);
                Console.WriteLine("Payment information adding started. ");
                var paymentInfo = new PaymentInfo { 
                    UserId = userId,
                    Amount = (float) paymentResponse.Data.Amount, 
                    Currency = paymentResponse.Data.Currency,
                    TxRef = transactionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                    };
                // Save the payment info to the database
                _context.PaymentInfos.Add(paymentInfo);
                await _context.SaveChangesAsync();
                Console.WriteLine("Payment information adding have finished. ");

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
