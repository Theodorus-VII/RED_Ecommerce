namespace Ecommerce.Controllers.Payment.Contracts
{
    public class PaymentRequestDTO
    {
        public string? ReturnUrl { get; set; }
        public string? Currency { get; set; }
        public string? PhoneNumber { get; set; }

    }

    public class SingleProductPaymentRequestDTO : PaymentRequestDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class PaymentVerifyDTO
    {
        public string? TxRef { get; set; }
        public int ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
    }

}
