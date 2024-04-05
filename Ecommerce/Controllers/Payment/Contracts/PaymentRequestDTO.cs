namespace Ecommerce.Controllers.Payment.Contracts
{
    public class PaymentRequestDTO
    {
        public float Amount { get; set; }
        public string? ReturnUrl { get; set; }
        public string? Currency { get; set; }
        public string? PhoneNumber { get; set; }
        public Customization? Customization { get; set; }

    }
    public record Customization
    {
        public string? ItemType { get; init; }
        public string? Id { get; init; }
    }

    public class PaymentVerifyDTO
    {
        public string? TxRef { get; set; }
    }

}
