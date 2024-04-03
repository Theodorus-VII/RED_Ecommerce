public class Address
{
    public int AddressId { get; set; } // Primary Key
    public string? UserId { get; set; } // Foreign Key referencing User Table
    public AddressType AddressType { get; set; } // Shipping or Billing
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}

public enum AddressType
{
    Shipping,
    Billing
}
