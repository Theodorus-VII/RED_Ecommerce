namespace Ecommerce.Models
{
    public class PaymentInfo
    {
        public int PaymentInfoId { get; set; }
        public string UserId { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        // Other payment-related properties as needed
    }
}
