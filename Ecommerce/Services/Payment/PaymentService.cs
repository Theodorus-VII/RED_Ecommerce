using ChapaNET;
using Ecommerce.Controllers.Payment.Contracts;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ecommerce.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly Chapa? _chapa;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string? _apikey = Environment.GetEnvironmentVariable("CHAPA_SECRET_KEY");
        private readonly IOrderService _orderService;

        public PaymentService(ApplicationDbContext context, IOrderService orderService)
        {
            if (_apikey != null)
            {
                _chapa = new Chapa(_apikey);
            }
            _context = context;
            _httpClient = new HttpClient();
            _orderService = orderService;
        }

        public HttpClient HttpClient => _httpClient;

        public async Task<PaymentResponseDTO> MakePaymentAsync(
            string userId,
            string email, 
            string firstName, 
            string lastName,
            string currency,
            string? returnUrl,
            string? phoneNumber)
        {
            try
            {


                var cart = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync() ?? throw new ArgumentException("Cart not found.");
                foreach (var cartItem in cart.Items)
                {
                    if (cartItem.Product != null)
                    {
                        int count = cartItem.Product.Count - cartItem.Quantity;
                        if (count < 0) { throw new ArgumentException($"{cartItem.Product.Name} out of stock"); }
                    }

                }
                var txRef = Chapa.GetUniqueRef();
                var chapaRequest = new ChapaRequest(
                    currency: currency,
                    amount: cart.TotalPrice, 
                    email: email, 
                    firstName: firstName, 
                    lastName: lastName, 
                    tx_ref: txRef, 
                    return_url: returnUrl,
                   phoneNo: phoneNumber);
                if (_chapa != null)
                { 
                    var result = await _chapa.RequestAsync(chapaRequest);
                    if (result.Status != "success")
                    {
                        throw new Exception("the request is not success.");
                    }
                    if (result.CheckoutUrl == null)
                    {
                        throw new Exception("checkout url is null.");
                    }
                    var paymentResponse = new PaymentResponseDTO { TransactionId = txRef, Url = result.CheckoutUrl };
                    return paymentResponse;
                }
                else
                {
                    throw new Exception("no chapa clinet.");
                }

            }
            catch (Exception)
            {
                throw;
            }   
            
            
        }


        public async Task<string> VerifyTransactionAsync(string userId, string TxRef, int shippingAddressId, int billingAddressId)
        {
            try
            {
                var apiUrl = "https://api.chapa.co/v1/transaction/verify/" + TxRef;
                var requesttochapa = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                requesttochapa.Headers.Add("Authorization", $"Bearer {_apikey}");
                var response = await HttpClient.SendAsync(requesttochapa);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("An error occurred while verifying payment");
                }
                else
                {
                    Console.WriteLine("Payment verified");
                }

                var content = await response.Content.ReadAsStringAsync() ?? throw new Exception("An error occurred while verifying payment");
                var paymentResponse = JsonConvert.DeserializeObject<dynamic>(content);

                if (paymentResponse?.data.status == "pending")
                {
                    throw new Exception("Payment is still pending. Please wait for confirmation.");
                }
                else if (paymentResponse?.data.status != "success")
                {
                    throw new Exception("The payment request was not successful.");
                }

                var paymentInfo = _context.PaymentInfos.FirstOrDefault(p => p.TxRef == TxRef);
                if (paymentInfo != null)
                {
                    throw new UnauthorizedAccessException("Incorrect txref.");
                }
                var newPaymentInfo = new PaymentInfo
                {
                    UserId = userId,
                    Amount = (float)paymentResponse.data.amount,
                    Currency = paymentResponse.data.currency,
                    TxRef = TxRef,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.PaymentInfos.Add(newPaymentInfo);
                await _context.SaveChangesAsync();
                var orderNo = await _orderService.MakeOrderAsync(userId, newPaymentInfo.PaymentInfoId, shippingAddressId, billingAddressId);
                return orderNo;
            }
            catch
            {
                throw;
            }
        }

    }

}
