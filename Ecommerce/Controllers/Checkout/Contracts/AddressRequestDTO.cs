namespace Ecommerce.Controllers.Checkout.Contracts
{
    public class AddressRequestDTO
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        // public Tuple<string, string>? Coordinates { get; set; }
    }

    public class UpdateAddressRequestDTO : AddressRequestDTO
    {
        public int AddressId { get; set; }
    }

    public class RemoveMultipleAddressRequestDTO
    {
        public List<int>? AddressIds { get; set; }
    }
}
