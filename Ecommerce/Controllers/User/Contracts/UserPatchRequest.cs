namespace Ecommerce.Controllers.Contracts;

public record UserPatchRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    
    string? OldPassword,
    string? NewPassword,
    string? DefaultShippingAddress,
    string? BillingAddress
);