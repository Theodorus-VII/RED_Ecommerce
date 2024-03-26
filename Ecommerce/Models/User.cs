using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DefaultShippingAddress { get; set; }
    public string? BillingAddress { get; set; }

    public User(
        string email,
        string firstName,
        string lastName,
        string? defaultShippingAddress,
        string? billingAddress
    )
    {
        Id = new Guid();
        UserName = email;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        DefaultShippingAddress = defaultShippingAddress;
        BillingAddress = billingAddress;
    }
}


public class UserDto
{
    public Guid Id {get;set;}
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? DefaultShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string Token { get; set; }

    public UserDto(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? defaultShippingAddress,
        string? billingAddress,
        string token)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        DefaultShippingAddress = defaultShippingAddress;
        BillingAddress = billingAddress;
        Token = token;
    }
    
    public UserDto(User user, string token)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        DefaultShippingAddress = user.DefaultShippingAddress;
        BillingAddress = user.BillingAddress;
        Token = token;
    }
}