using System.Security.Claims;
using System.Text;
using AutoMapper;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Identity;

// temp
using FirebaseAdmin.Messaging;

namespace Ecommerce.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserAccountService _userAccountService;
    private readonly SignInManager<User> _signInManager;

    public AuthService(
        UserManager<User> userManager,
        IJwtTokenGenerator tokenGenerator,
        IMapper mapper,
        IEmailService emailService,
        IUserAccountService userAccountService,
        ILogger<AuthService> logger,
        SignInManager<User> signInManager
    )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _userAccountService = userAccountService;
        _logger = logger;
        _mapper = mapper;
    }

    private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
    {
        return _tokenGenerator.GetPrincipalFromExpiredToken(token);
    }

    public async Task<IServiceResponse<UserDto>> LoginUser(string email, string password, string? fcmToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: true,
            lockoutOnFailure: false);

        _logger.LogInformation("{}", result.Succeeded);
        if (!result.Succeeded)
        {
            _logger.LogInformation("{}", result);
            if (user == null) // user not found.
            {
                return ServiceResponse<UserDto>.FailResponse(
                    statusCode: StatusCodes.Status404NotFound,
                    errorDescription: "User Not Found"
                );
            }
            if (!user.EmailConfirmed) // user has not yet confirmed their email.
            {
                return ServiceResponse<UserDto>.FailResponse(
                    statusCode: StatusCodes.Status401Unauthorized,
                    errorDescription: "Please confirm your email first before logging into the service."
                );
            }
            if (!await _userManager.CheckPasswordAsync(user, password)) // incorrect password
            {
                return ServiceResponse<UserDto>.FailResponse(
                    statusCode: StatusCodes.Status401Unauthorized,
                    errorDescription: "Invalid Password"
                );
            }

            // some other error encountered. return status 500.
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status500InternalServerError,
                errorDescription: "Internal Server Error Encountered Signing in. Please try again later or contact support if the issue persists."
            );
        }

        // Set the fcm token of the user.
        await SetFCMToken(email, fcmToken);

        // Using SignInManager to create the claims. Could also do this manually, but this is more concise and cleaner.
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        var claims = principal.Claims.ToList();

        // Generate the access and refresh tokens for the user.
        string accessToken = _tokenGenerator.GenerateToken(user, claims);
        string refreshToken = _tokenGenerator.GenerateRefreshToken();

        // Set the refresh token in the database.
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        // Get the user's role for the response.
        string userRole = await _userAccountService.GetUserRole(user);

        _logger.LogInformation("UserRole: {}", userRole);

        // Section: 
        //  testing push notifications. Since we set the fcm token here, it's easier to just send one out to test using it here.

        try
        {
            _logger.LogInformation("Attempting to send push notificaiton");
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "You have successfully logged in.",
                    Body = "This is just a demo push notification."
                },
                Token = fcmToken
            };
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Message successfully sent: {}", response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error sending push notification: {}", e);
        }
        // endSection

        return ServiceResponse<UserDto>.SuccessResponse(
            statusCode: StatusCodes.Status200OK,
            data: new UserDto(
                user: user,
                accessToken: accessToken,
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

        // Set the refresh token in the database to an empty string (remove it if exists).
        user.RefreshToken = "";
        user.FCMToken = null;
        await _userManager.UpdateAsync(user);

        return ServiceResponse<bool>.SuccessResponse(
            statusCode: 200,
            data: true
        );
    }

    public async Task<IServiceResponse<UserDto>> RegisterAdmin(
        User user, string password)
    {
        return await RegisterUser(user, password, Roles.Admin);
    }

    public async Task<IServiceResponse<UserDto>> RegisterCustomer(
        User user, string password)
    {
        return await RegisterUser(user, password, Roles.Customer);
    }

    private async Task<IServiceResponse<UserDto>> RegisterUser(
        User user, string password, string Role)
    {
        // check if user alerady exists.
        if (await _userManager.FindByEmailAsync(user.Email) != null)
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status409Conflict,
                errorDescription: "Email already in use"
            );
        }

        _logger.LogInformation("Creating user...");

        var result = await _userManager.CreateAsync(user, password);

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

    public async Task<IServiceResponse<UserDto>> RefreshToken(
        string expiredToken, string refreshToken)
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

        // Update the user in the db.
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
        if (user.EmailConfirmed)
        {
            // Email Already Confirmed.
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorDescription: "User email already confirmed"
            );
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            _logger.LogInformation("Email confirmed...");
            return ServiceResponse<bool>.SuccessResponse(
                statusCode: 200,
                data: true);
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
                errorDescription: "Error Confirming Email. Please try again later."
            );
        }
    }

    public async Task<IServiceResponse<bool>> SendConfirmationEmail(
        User user,
        string baseUrl,
        string scheme,
        string callbackUrl,
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
        _logger.LogInformation(
            "Generated Confirmation Token: {}",
            stringConfirmationToken);

        var encodedConfirmationToken = System.Web.HttpUtility.UrlEncode(stringConfirmationToken);

        _logger.LogInformation(
            "Encoded Generated confirmation Token: {}",
            encodedConfirmationToken);

        var email_callbackURL =
            $"{scheme}://{baseUrl}{action}?userId={user.Id}&token={encodedConfirmationToken}&callbackUrl={callbackUrl}";

        var confirmationEmail = new EmailDto
        {
            Recipient = user.Email,
            Subject = "Welcome to _______ Commerce",
            Message =
                $@"
                    <p>
                    Your new account at  _______ Commerce has been created. Please 
                    confirm your account by <a href={email_callbackURL}>clicking here.</a>
                    </p>
                "
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
        string callbackUrl,
        string action = "reset-password"
    )
    {
        try
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Unencoded Reset Token: {}", resetToken);

            resetToken = System.Web.HttpUtility.UrlEncode(resetToken, Encoding.UTF8);
            _logger.LogInformation("Encoded Reset Token: {}", resetToken);

            var service_callbackUrl =
                $"{scheme}://{baseUrl}{action}?email={user.Email}&token={resetToken}&callbackUrl={callbackUrl}";
            var passResetEmail = new EmailDto
            {
                Recipient = user.Email,
                Subject = "Reset Password",
                Message = $@"<p>Reset your password <a href={service_callbackUrl}>here</a></p>"
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
                data: "Password Changed. Please login using your new password."
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

    public async Task<IServiceResponse<bool>> SetFCMToken(string email, string? fcmToken)
    {
        try
        {
            if (fcmToken != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                _logger.LogInformation("Old fcm token: {}, New fcm token: {}", user.FCMToken, fcmToken);
                user.FCMToken = fcmToken;
                await _userManager.UpdateAsync(user);
            }
            return ServiceResponse<bool>.SuccessResponse(
                statusCode: StatusCodes.Status200OK,
                data: true
            );
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e.ToString());
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status500InternalServerError,
                errorDescription: $"Error while setting fcm token: {e.ToString()}"
            );
        }
    }
}
