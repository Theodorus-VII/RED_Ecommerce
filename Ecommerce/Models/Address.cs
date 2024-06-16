namespace Ecommerce.Models
{
    public class ShippingAddress
    {
        public int ShippingAddressId { get; set; } 
        public string? UserId { get; set; } // Shipping or Billing
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class BillingAddress
    {
        public int BillingAddressId { get; set; }
        public string? UserId { get; set; } // Shipping or Billing
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}