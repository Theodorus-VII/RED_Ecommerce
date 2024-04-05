namespace Ecommerce.Models
{
    public class PaymentResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public PaymentData Data { get; set; }
    }

    public class PaymentData
    {
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public object Phone_number { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Charge { get; set; }
        public string Mode { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Tx_ref { get; set; }
        public Customization Customization { get; set; }
        public object Meta { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }

    public class Customization
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
    }

}
