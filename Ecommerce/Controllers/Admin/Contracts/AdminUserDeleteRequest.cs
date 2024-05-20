namespace Ecommerce.Controllers.Contracts;

public record AdminUserDeleteRequest(
    string? UserId,
    string? Email
);