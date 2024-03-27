namespace Ecommerce.Controllers.Contracts;

public record TokenRequestModel(
    string AccessToken,
    string RefreshToken
);