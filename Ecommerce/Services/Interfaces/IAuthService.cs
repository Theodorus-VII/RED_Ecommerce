using System.Security.Policy;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Services;

public interface IAuthService
{
    public IEnumerable<User> GetUsers();
    public Task<IAuthResponse> RegisterCustomer(RegistrationRequest request);
    public Task<IAuthResponse> RegisterAdmin(RegistrationRequest request);
    public Task<string?> GenerateEmailConfirmationToken(string email);

    public Task<bool> SendConfirmationEmail(
        UserDto user,
        string baseUrl,
        string scheme,
        string action
    );
    public Task<bool> ConfirmEmail(string email, string token);
    public Task<IAuthResponse> LoginUser(LoginRequest request);
    public Task<bool> LogoutUser(string userId);
    public Task<IAuthResponse> RefreshToken(string expiredToken, string refreshToken);
}
