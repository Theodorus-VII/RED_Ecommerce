using System.Security.Cryptography.X509Certificates;

namespace Ecommerce.Controllers.Contracts;

public record UserDeleteRequest(
    string? userId
);