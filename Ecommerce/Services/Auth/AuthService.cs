using System.Security.Claims;
using System.Text;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserAccountService _userAccountService;

    public AuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IJwtTokenGenerator tokenGenerator,
        IEmailService emailService,
        IUserAccountService userAccountService,
        ILogger<AuthService> logger
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _userAccountService = userAccountService;
        _logger = logger;
    }

    public IEnumerable<User> GetUsers()
    {
        return _userManager.Users;
    }

    private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
    {
        return _tokenGenerator.GetPrincipalFromExpiredToken(token);
    }

    public async Task<IServiceResponse<UserDto>> LoginUser(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status401Unauthorized,
                errorDescription: "Invalid Password"
            );
        }

        var claims = new List<Claim>();
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        string token = _tokenGenerator.GenerateToken(user, claims);
        string refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        string userRole = await _userAccountService.GetUserRole(user);

        _logger.LogInformation("UserRole: {}", userRole);

        return ServiceResponse<UserDto>.SuccessResponse(
            statusCode: StatusCodes.Status200OK,
            data: new UserDto(
                user: user,
                accessToken: token,
                refreshToken: refreshToken,
                role: userRole
            )
        );
    }

    public async Task<IServiceResponse<bool>> LogoutUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }

        user.RefreshToken = "";
        await _userManager.UpdateAsync(user);

        return ServiceResponse<bool>.SuccessResponse(
            statusCode: 200,
            data: true
        );
    }

    public async Task<IServiceResponse<UserDto>> RegisterAdmin(RegistrationRequest request)
    {
        return await RegisterUser(request, Roles.Admin);
    }

    public async Task<IServiceResponse<UserDto>> RegisterCustomer(RegistrationRequest request)
    {
        return await RegisterUser(request, Roles.Customer);
    }

    public async Task<IServiceResponse<UserDto>> RegisterUser(RegistrationRequest request, string Role)
    {
        // check if user alerady exists.
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status409Conflict,
                errorDescription: "Email already in use"
            );
        }

        _logger.LogInformation("Creating user...");

        User user = new User(
            request.Email,
            request.FirstName,
            request.LastName,
            request.DefaultShippingAddress,
            request.BillingAddress
        );

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("Encountered Identity Errors: {}", result.Errors);
            throw new Exception("Server Error");
        }

        _logger.LogInformation("User added to server.");

        var assignRoleResult = await _userManager.AddToRoleAsync(user, Role);

        if (!assignRoleResult.Succeeded)
        {
            // non terminal error so proceeding. logging for debugging purposes.
            _logger.LogError(
                "Encountered Role Assignment Error: {}",
                assignRoleResult.Errors);
        }

        var claims = new List<Claim>();
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            _logger.LogInformation("User Role: {}", role);
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        string token = _tokenGenerator.GenerateToken(user, claims);
        string refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        var userRole = await _userAccountService.GetUserRole(user);
        return ServiceResponse<UserDto>.SuccessResponse(
            statusCode: StatusCodes.Status201Created,
            data: new UserDto(
                user: user,
                accessToken: token,
                refreshToken: refreshToken,
                role: userRole)
        );
    }

    public async Task<IServiceResponse<UserDto>> RefreshToken(string expiredToken, string refreshToken)
    {
        _logger.LogInformation("Refreshing User Token");

        // extract user data from the expired token
        var principal = GetClaimsPrincipalFromExpiredToken(expiredToken);
        var userIdClaim = principal.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier
        );

        _logger.LogInformation("User ID Claim: {}", userIdClaim);

        // return error("Invalid User. Claim corresponding to Id not found");
        if (userIdClaim is null)
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found");
        }

        Guid userId = Guid.Parse(userIdClaim.Value);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        _logger.LogInformation("User Found: {}", user);
        // return error if the user found is null.
        if (user is null)
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found");
        }

        // Errors due to invalid refresh/access tokens.
        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
        {
            _logger.LogDebug($"User refresh token: {user.RefreshToken}");
            _logger.LogDebug($"Supplied refresh token: {refreshToken}");
            _logger.LogDebug($"Token Expiry: {user.RefreshTokenExpiry}");

            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status401Unauthorized,
                errorDescription: "Invalid or Expired Refresh Token");
        }

        // Generate new access and refresh tokens.
        var newAccessToken = _tokenGenerator.GenerateToken(user, principal.Claims);

        // Save the refresh token to the db.
        var newRefreshToken = _tokenGenerator.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        var role = await _userAccountService.GetUserRole(user);

        return ServiceResponse<UserDto>.SuccessResponse(
            statusCode: 200,
            data: new UserDto(
                user: user,
                accessToken: newAccessToken,
                refreshToken: newRefreshToken,
                role: role)
            );

    }

    public async Task<IServiceResponse<string>> GenerateEmailConfirmationToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var response = new ServiceResponse<string>();

        if (confirmationToken == null)
        {
            return ServiceResponse<string>.FailResponse(
                statusCode: 500,
                errorDescription: "Internal Server Error Generating Confirmation Token"
            );
        }

        return ServiceResponse<string>.SuccessResponse(
            statusCode: 200,
            data: confirmationToken
        );
    }

    public async Task<IServiceResponse<bool>> ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return ServiceResponse<bool>.SuccessResponse(statusCode: 200, data: true);
        }
        else
        {
            _logger.LogError("Identity Error confirming email");
            _logger.LogError(result.Errors.ToArray().ToString());
            foreach (var error in result.Errors)
            {
                _logger.LogError(error.ToString());
            }

            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status500InternalServerError,
                errorDescription: "Error Confirming Email"
            );
        }
    }

    public async Task<IServiceResponse<bool>> SendConfirmationEmail(
        UserDto user,
        string baseUrl,
        string scheme,
        string action = "confirm-email"
    )
    {
        _logger.LogInformation("Sending Confirmation Email...");

        var generateConfirmationToken = await GenerateEmailConfirmationToken(user.Email);
        if (!generateConfirmationToken.IsSuccess)
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: generateConfirmationToken.Error.ErrorCode,
                errorDescription: generateConfirmationToken.Error.ErrorDescription
            );
        }

        var stringConfirmationToken = generateConfirmationToken.Data;

        var encodedConfirmationToken = System.Web.HttpUtility.UrlEncode(stringConfirmationToken, Encoding.UTF8);
        var callbackUrl =
            $"{scheme}://{baseUrl}{action}?userId={user.Id}&token={encodedConfirmationToken}";

        var confirmationEmail = new EmailDto
        {
            Recipient = user.Email,
            Subject = "Welcome to _______ Commerce",
            Message =
                $@"<p>Your new account at  _______Commerce has been created. 
                    Please confirm your account by <a href={callbackUrl}>clicking here.</a></p>"
        };

        _logger.LogInformation($"URL for email confirmation: {callbackUrl}");

        var result = await _emailService.SendEmail(confirmationEmail);

        if (result.IsSuccess)
        {
            return ServiceResponse<bool>.SuccessResponse(
                statusCode: StatusCodes.Status200OK,
                data: true
            );
        }
        return ServiceResponse<bool>.FailResponse(
            statusCode: StatusCodes.Status500InternalServerError,
            errorDescription: "Error sending account confirmation email"
        );
    }

    public async Task<IServiceResponse<string>> SendPasswordResetEmail(
        User user,
        string baseUrl,
        string scheme,
        string action = "reset-password"
    )
    {
        try
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Unencoded Reset Token: {}", resetToken);

            resetToken = System.Web.HttpUtility.UrlEncode(resetToken, Encoding.UTF8);
            _logger.LogInformation("Encoded Reset Token: {}", resetToken);

            var callbackUrl = $"{scheme}://{baseUrl}{action}?email={user.Email}&token={resetToken}";
            var passResetEmail = new EmailDto
            {
                Recipient = user.Email,
                Subject = "Reset Password",
                Message = $@"<p>Reset your password <a href={callbackUrl}>here</a></p>"
            };

            _logger.LogInformation("Password reset Email sending...");
            
            await _emailService.SendEmail(passResetEmail);
            
            _logger.LogInformation("Password Reset Email Sent");
            return ServiceResponse<string>.SuccessResponse(
                statusCode: StatusCodes.Status200OK,
                data: "Confirmation Email Sent"
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while sending password reset email: {e}");

            return ServiceResponse<string>.FailResponse(
                statusCode: StatusCodes.Status500InternalServerError,
                errorDescription: "Server Error while sending password reset email"
            );
        }
    }

    public async Task<IServiceResponse<string>> ResetPassword(
        string email,
        string resetToken,
        string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogError("User does not exist");

            return ServiceResponse<string>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }

        _logger.LogInformation("Resetting password...");
        _logger.LogInformation("Token: {}, Password: {}", resetToken, newPassword);
        var result =
            await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (result.Succeeded)
        {
            return ServiceResponse<string>.SuccessResponse(
                statusCode: StatusCodes.Status200OK,
                data: "Password Reset Email Sent Successfully"
            );
        }

        _logger.LogError("Error resetting user password.");
        foreach (var error in result.Errors)
        {
            _logger.LogError(error.ToString());
        }

        return ServiceResponse<string>.FailResponse(
            statusCode: StatusCodes.Status500InternalServerError,
            errorDescription: "Server Error while resetting user password."
        );
    }
}
