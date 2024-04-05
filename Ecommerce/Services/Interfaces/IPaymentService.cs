using Ecommerce.Controllers.Payment.Contracts;

namespace Ecommerce.Services.Interfaces
{
    public interface IPaymentService 
    {
        Task<Object> MakePaymentAsync(PaymentRequestDTO paymentRequest, string email, string firstName, string lastName);
        Task VerifyTransactionAsync(string userId, string transactionId);
    }
}
