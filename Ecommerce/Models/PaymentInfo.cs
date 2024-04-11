namespace Ecommerce.Models
{
    public class PaymentInfo
    {
        public int PaymentInfoId { get; set; }
        public string? UserId { get; set; }
        public float Amount { get; set; }
        public string? Currency { get; set; }    
        public string? TxRef { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
