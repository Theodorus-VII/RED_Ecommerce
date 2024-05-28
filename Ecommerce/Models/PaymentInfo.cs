namespace Ecommerce.Models
{
    public class PaymentInfo
    {
        public int PaymentInfoId { get; set; }
        public string? UserId { get; set; }
        public double Amount { get; set; }
        public string? Currency { get; set; }    
        public string? TxRef { get; set; }
        public int? ProductId { get; set; } = null;
        public bool Verified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
