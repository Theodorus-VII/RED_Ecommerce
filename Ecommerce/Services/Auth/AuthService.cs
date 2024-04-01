using System.Security.Claims;
using System.Security.Policy;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IJwtTokenGenerator tokenGenerator,
        IEmailService emailService,
        ILogger<AuthService> logger
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public IEnumerable<User> GetUsers()
    {
        return _userManager.Users;
    }

    public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
    {
        return _tokenGenerator.GetPrincipalFromExpiredToken(token);
    }

    public async Task<IAuthResponse> LoginUser(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new Exception("Invalid Password");
            }

            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                _logger.LogInformation($"User Role: {role}");
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            string token = _tokenGenerator.GenerateToken(user, claims);
            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthSucessResponse(new UserDto(user, token, refreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Login Error: {e}");
            return new AuthFailResponse(error: "Invalid Username or Password");
        }
    }

    public async Task<bool> LogoutUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }
        user.RefreshToken = "";
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<IAuthResponse> RegisterAdmin(RegistrationRequest request)
    {
        return await RegisterUser(request, Roles.Admin);
    }

    public async Task<IAuthResponse> RegisterCustomer(RegistrationRequest request)
    {
        return await RegisterUser(request, Roles.Customer);
    }

    public async Task<IAuthResponse> RegisterUser(RegistrationRequest request, string Role)
    {
        try
        {
            // check if user alerady exists.
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                throw new Exception("Email already exists.");
            }
            _logger.LogInformation("Creating user...");
            User user =
                new(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.DefaultShippingAddress,
                    request.BillingAddress
                );
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"Encountered Identity Errors: {result.Errors}");
                throw new Exception("Server Error");
            }
            _logger.LogInformation("User added to server.");
            var assignRoleResult = await _userManager.AddToRoleAsync(user, Role);
            if (!assignRoleResult.Succeeded)
            {
                _logger.LogError($"Encountered Role Assignment Error: {assignRoleResult.Errors}");
            }

            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                _logger.LogInformation($"User Role: {role}");
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            string token = _tokenGenerator.GenerateToken(user, claims);
            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthSucessResponse(new UserDto(user, token, refreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Registration Error: {e}");
            return new AuthFailResponse(e.Message);
        }
    }

    public async Task<IAuthResponse> RefreshToken(string expiredToken, string refreshToken)
    {
        try
        {
            // extract user data from the expired token
            var principal = GetClaimsPrincipalFromExpiredToken(expiredToken);

            var userIdClaim = principal.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier
            );

            _logger.LogInformation($"{userIdClaim}");
            if (userIdClaim is null)
            {
                throw new Exception("Invalid User. Claim corresponding to Id not found");
            }

            Guid userId = Guid.Parse(userIdClaim.Value);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            _logger.LogInformation($"{user}");

            if (user is null)
            {
                throw new Exception("Invalid user");
            }
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                _logger.LogInformation($"User refresh token: {user.RefreshToken}");
                _logger.LogInformation($"Supplied refresh token: {refreshToken}");
                _logger.LogInformation($"Token Expiry: {user.RefreshTokenExpiry}");

                throw new Exception("Invalid or Expired Refresh Token");
            }
            var newAccessToken = _tokenGenerator.GenerateToken(user, principal.Claims);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthSucessResponse(new UserDto(user, newAccessToken, newRefreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Refresh Token Error: {e}");
            return new AuthFailResponse("Invalid Client Request");
        }
    }

    public async Task<string?> GenerateEmailConfirmationToken(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error encountered while creating email confirmation token: {e}");
            return null;
        }
    }

    public async Task<bool> ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return true;
        }
        else
        {
            _logger.LogError("Identity Error confirming email");
            _logger.LogError(result.Errors.ToArray().ToString());
            foreach (var error in result.Errors)
            {
                _logger.LogError(error.ToString());
            }
        }
        return false;
    }

    public async Task<bool> SendConfirmationEmail(
        UserDto user,
        string baseUrl,
        string scheme,
        string action = "confirm-email"
    )
    {
        try
        {
            _logger.LogInformation("Sending Confirmation Email...");

            var confirmationToken = await GenerateEmailConfirmationToken(user.Email);
            confirmationToken = System.Web.HttpUtility.UrlEncode(confirmationToken);
            var callbackUrl =
                $"{scheme}://{baseUrl}{action}?userId={user.Id}&token={confirmationToken}";

            var confirmationEmail = new EmailDto
            {
                Recipient = user.Email,
                Subject = "Welcome to _______ Commerce",
                Message =
                    $@"<p>Your new account at  _______Commerce has been created. 
                    Please confirm your account by <a href={callbackUrl}>clicking here.</a></p>"
            };

            _logger.LogInformation($"URL for email confirmation: {callbackUrl}");
            await _emailService.SendEmail(confirmationEmail);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error sending confirmatio email: {e}");
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmail(
        User user,
        string baseUrl,
        string scheme,
        string action = "reset-password"
    )
    {
        try
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            resetToken = System.Web.HttpUtility.UrlEncode(resetToken);
            var callbackUrl = $"{scheme}://{baseUrl}{action}?userId={user.Id}&token={resetToken}";

            var passResetEmail = new EmailDto
            {
                Recipient = user.Email,
                Subject = "Reset Password",
                Message = $@"<p>Reset your password <a href={callbackUrl}>here</a></p>"
            };
            _logger.LogInformation("Password reset Email sending...");
            await _emailService.SendEmail(passResetEmail);
            _logger.LogInformation("Password Reset Email Sent");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while sending password reset email: {e}");
            return false;
        }
    }

    public async Task<bool> ResetPassword(string email, string resetToken, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogError("User does not exist");
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (result.Succeeded)
        {
            return true;
        }

        _logger.LogError("Error resetting user password");
        foreach (var error in result.Errors)
        {
            _logger.LogError(error.ToString());
        }
        return false;
    }
}
