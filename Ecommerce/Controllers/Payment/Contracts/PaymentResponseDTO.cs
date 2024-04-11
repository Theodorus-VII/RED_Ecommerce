namespace Ecommerce.Controllers.Payment.Contracts
{
    public class PaymentResponseDTO
    {
        public string TransactionId { get; set; } = null!;
        public string Url { get; set; } = null!;
    }
}
