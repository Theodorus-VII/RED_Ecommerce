namespace Ecommerce.Models
{
    public class Address
    {
        public int AddressId { get; set; } 
        public string? UserId { get; set; } 
        public AddressType AddressType { get; set; } // Shipping or Billing
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
}