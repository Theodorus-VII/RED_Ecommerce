using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers.Contracts;

public record RegistrationRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? DefaultShippingAddress,
    string? BillingAddress
);