namespace Ecommerce.Controllers.Contracts;

public record LoginRequest(
    string Email,
    string Password
    );