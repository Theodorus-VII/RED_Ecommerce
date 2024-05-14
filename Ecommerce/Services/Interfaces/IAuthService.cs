using System.Security.Policy;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Utilities;

namespace Ecommerce.Services;

public interface IAuthService
{
    public Task<IServiceResponse<UserDto>> RegisterCustomer(User user, string password);
    public Task<IServiceResponse<UserDto>> RegisterAdmin(User user, string password);
    public Task<IServiceResponse<UserDto>> LoginUser(string email, string password);
    public Task<IServiceResponse<bool>> LogoutUser(string userId);
    public Task<IServiceResponse<UserDto>> RefreshToken(string expiredToken, string refreshToken);
    public Task<IServiceResponse<string>> GenerateEmailConfirmationToken(string email);
    public Task<IServiceResponse<bool>> ConfirmEmail(string email, string token);
    public Task<IServiceResponse<bool>> SendConfirmationEmail(
        User user,
        string baseUrl,
        string scheme,
        string callbackUrl,
        string action
    );
    public Task<IServiceResponse<string>> SendPasswordResetEmail(
        User user,
        string baseUrl,
        string scheme,
        string callbackUrl,
        string action
    );
    public Task<IServiceResponse<string>> ResetPassword(string email, string resetToken, string newPassword);
}
