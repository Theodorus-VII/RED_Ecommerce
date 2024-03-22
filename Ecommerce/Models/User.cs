using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models;

public class User : IdentityUser<Guid>{
    public string? FirstName {get; set;}
    public string? LastName {get; set;}
    public string? DefaultShippingAddress {get; set;}
    public string? BillingAddress {get; set;}
}