using System.Security.Policy;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Utilities;

namespace Ecommerce.Services;

public interface IAuthService
{
    public Task<IEnumerable<UserDto>> GetUsers();
    public Task<IServiceResponse<UserDto>> RegisterCustomer(RegistrationRequest request);
    public Task<IServiceResponse<UserDto>> RegisterAdmin(RegistrationRequest request);

    public Task<IServiceResponse<UserDto>> LoginUser(LoginRequest request);
    public Task<IServiceResponse<bool>> LogoutUser(string userId);
    public Task<IServiceResponse<UserDto>> RefreshToken(string expiredToken, string refreshToken);
    public Task<IServiceResponse<string>> GenerateEmailConfirmationToken(string email);
    public Task<IServiceResponse<bool>> ConfirmEmail(string email, string token);
    public Task<IServiceResponse<bool>> SendConfirmationEmail(
        UserDto user,
        string baseUrl,
        string scheme,
        string action
    );
    public Task<IServiceResponse<string>> SendPasswordResetEmail(
        User user,
        string baseUrl,
        string scheme,
        string action
    );
    public Task<IServiceResponse<string>> ResetPassword(string email, string resetToken, string newPassword);
}
