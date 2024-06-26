using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DefaultShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public string? FCMToken { get; set; } = "";// Firebase Cloud Messaging Token

    public override DateTimeOffset? LockoutEnd
    {
        get => base.LockoutEnd?.ToUniversalTime();
        set => base.LockoutEnd = value?.ToUniversalTime();
    }


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

    public User(
        string email,
        string firstName,
        string lastName
    )
    {
        Id = new Guid();
        UserName = email;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public User UpdateUser(
        string? email,
        string? firstName,
        string? lastName,
        string? defaultShippingAddress,
        string? billingAddress,
        string? phoneNumber
    )
    {
        Email = email ?? Email;
        UserName = email ?? UserName;
        FirstName = firstName ?? FirstName;
        LastName = lastName ?? LastName;
        BillingAddress = billingAddress ?? BillingAddress;
        DefaultShippingAddress =
            defaultShippingAddress ?? DefaultShippingAddress;
        PhoneNumber = phoneNumber ?? PhoneNumber;
        return this;
    }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? DefaultShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string? FCMToken { get; set; } = "";

    public UserDto(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? defaultShippingAddress,
        string? billingAddress,
        string accessToken,
        string refreshToken,
        string role
    )
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        DefaultShippingAddress = defaultShippingAddress;
        BillingAddress = billingAddress;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Role = role;
    }

    public UserDto(
        User user,
        string accessToken,
        string refreshToken,
        string role)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        DefaultShippingAddress = user.DefaultShippingAddress;
        BillingAddress = user.BillingAddress;
        PhoneNumber = user.PhoneNumber;
        FCMToken = user.FCMToken;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Role = role;
    }

    public UserDto(User user, string role)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        DefaultShippingAddress = user.DefaultShippingAddress;
        BillingAddress = user.BillingAddress;
        PhoneNumber = user.PhoneNumber;
        Role = role;
        // FCMToken = user.FCMToken;
    }

    public UserDto(string firstName, string lastName, string email)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}
