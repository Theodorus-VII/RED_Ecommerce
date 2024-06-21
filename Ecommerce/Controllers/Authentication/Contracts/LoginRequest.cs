using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers.Contracts;

public record LoginRequest(
    [Required(ErrorMessage = "Email must be provided")] string Email,
    [Required(ErrorMessage = "Password must be provided")] string Password,
    string? FCMToken
);
