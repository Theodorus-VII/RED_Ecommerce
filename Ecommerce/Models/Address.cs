namespace Ecommerce.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        // Other address-related properties as needed
    }
}
