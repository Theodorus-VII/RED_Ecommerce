using Ecommerce.Controllers.Payment.Contracts;

namespace Ecommerce.Services.Interfaces
{
    public interface IPaymentService 
    {
        Task<PaymentResponseDTO> MakePaymentAsync(
            string userId,
            string email,
            string firstName,
            string lastName,
            string currency,
            string returnUrl,
            string? phoneNumber,
            int? productId,
            int? quantity);
        Task<string> VerifyTransactionAsync(string userId, string transactionId, int shippingAddressId, int? billingAddressId);
    }
}
