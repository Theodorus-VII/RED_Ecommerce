using System.Security.Claims;
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

    public async Task<IServiceResponse<UserDto>> LoginUser(LoginRequest request)
    {
        var response = new ServiceResponse<UserDto>();
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.Error = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorDescription = "User not found"
                };
                response.IsSuccess = false;

                return response;
            }
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                response.Error = new ErrorResponse()
                {
                    ErrorCode = 401,
                    ErrorDescription = "Invalid Password"
                };
                response.IsSuccess = false;

                return response;
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
            response.Data = new UserDto(user, token, refreshToken);
            response.StatusCode = 200;
            response.IsSuccess = true;

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Login Error: {e}");
            response.StatusCode = 500;
            response.Error = new ErrorResponse()
            {
                ErrorCode = 500,
                ErrorDescription = e.Message
            };
            response.IsSuccess = false;

            return response;
        }
    }

    public async Task<IServiceResponse<bool>> LogoutUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new ServiceResponse<bool>()
            {
                Data = false,
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorDescription = "User not found."
                }
            };
        }
        user.RefreshToken = "";
        await _userManager.UpdateAsync(user);

        return new ServiceResponse<bool>()
        {
            Data = true,
            IsSuccess = true,
        };
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
        try
        {
            // check if user alerady exists.
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                // throw new Exception("Email already exists.");
                return new ServiceResponse<UserDto>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 409,
                        ErrorDescription = "Email already in use"
                    }
                };
            }
            _logger.LogInformation("Creating user...");
            User user = new(
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
                _logger.LogError("Encountered Role Assignment Error: {}", assignRoleResult.Errors);
                // non terminal so proceeding.
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

            // return new AuthSucessResponse(new UserDto(user, token, refreshToken));
            return new ServiceResponse<UserDto>()
            {
                Data = new UserDto(user, token, refreshToken),
                IsSuccess = true,
                StatusCode = 201
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"Registration Error: {e}");
            // return new AuthFailResponse(e.Message);
            return new ServiceResponse<UserDto>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Internal Server Error"
                }
            };
        }
    }

    public async Task<IServiceResponse<UserDto>> RefreshToken(string expiredToken, string refreshToken)
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
                // throw new Exception("Invalid User. Claim corresponding to Id not found");
                return new ServiceResponse<UserDto>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 404,
                        ErrorDescription = "User Not Found"
                    }
                };
            }

            Guid userId = Guid.Parse(userIdClaim.Value);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            _logger.LogInformation($"{user}");

            if (user is null)
            {
                return new ServiceResponse<UserDto>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 404,
                        ErrorDescription = "User Not Found"
                    }
                };
            }
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                _logger.LogDebug($"User refresh token: {user.RefreshToken}");
                _logger.LogDebug($"Supplied refresh token: {refreshToken}");
                _logger.LogDebug($"Token Expiry: {user.RefreshTokenExpiry}");

                return new ServiceResponse<UserDto>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 401,
                        ErrorDescription = "Invalid or Expired Refresh Token"
                    }
                };
            }
            var newAccessToken = _tokenGenerator.GenerateToken(user, principal.Claims);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            // return new AuthSucessResponse(new UserDto(user, newAccessToken, newRefreshToken));
            return new ServiceResponse<UserDto>()
            {
                Data = new UserDto(user, newAccessToken, newRefreshToken),
                IsSuccess = true
            };
        }

        catch (Exception e)
        {
            _logger.LogError($"Refresh Token Error: {e}");
            // return new AuthFailResponse("Invalid Client Request");
            return new ServiceResponse<UserDto>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Internal Server Error"
                }
            };
        }
    }

    public async Task<IServiceResponse<string>> GenerateEmailConfirmationToken(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var response = new ServiceResponse<string>();

            if (confirmationToken == null)
            {
                response.StatusCode = 500;
                response.Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Internal Server Error Generating Confirmation Token"
                };
                return response;
            }
            response.StatusCode = 200;
            response.Data = confirmationToken;

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error encountered while creating email confirmation token: {e}");
            return new ServiceResponse<string>()
            {
                StatusCode = 500,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Internal Server Error Generating Confirmation Token"
                }
            };
        }
    }

    public async Task<IServiceResponse<bool>> ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new ServiceResponse<bool>()
            {
                Data = false,
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorDescription = "User Not Found"
                }
            };
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return new ServiceResponse<bool>()
            {
                Data = true,
                IsSuccess = true,
                StatusCode = 200
            };
        }
        else
        {
            _logger.LogError("Identity Error confirming email");
            _logger.LogError(result.Errors.ToArray().ToString());
            foreach (var error in result.Errors)
            {
                _logger.LogError(error.ToString());
            }

            return new ServiceResponse<bool>()
            {
                Data = false,
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Error confirming email"
                }
            };
        }
    }

    public async Task<IServiceResponse<bool>> SendConfirmationEmail(
        UserDto user,
        string baseUrl,
        string scheme,
        string action = "confirm-email"
    )
    {
        try
        {
            _logger.LogInformation("Sending Confirmation Email...");

            var genConfirmationToken = await GenerateEmailConfirmationToken(user.Email);
            if (!genConfirmationToken.IsSuccess)
            {
                return new ServiceResponse<bool>()
                {
                    IsSuccess = false,
                    Error = genConfirmationToken.Error
                };
            }
            var stringConfirmationToken = genConfirmationToken.Data;

            var encodedConfirmationToken = System.Web.HttpUtility.UrlEncode(stringConfirmationToken);
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
            await _emailService.SendEmail(confirmationEmail);
            return new ServiceResponse<bool>()
            {
                IsSuccess = true,
                Data = true
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error sending confirmation email: {e}");
            return new ServiceResponse<bool>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = $"Error sending confirmation email: {e}"
                }
            };
        }
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
            return new ServiceResponse<string>()
            {
                StatusCode = 200,
                IsSuccess = true,
                Data = "Confirmation Email sent"
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while sending password reset email: {e}");
            return new ServiceResponse<string>()
            {
                Data = "Confirmation email not sent",
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Server Error while sending password reset email"
                }
            };
        }
    }

    public async Task<IServiceResponse<string>> ResetPassword(string email, string resetToken, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogError("User does not exist");
            return new ServiceResponse<string>()
            {
                IsSuccess = false,
                Data = "User Not found",
                Error = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorDescription = "User not found"
                }
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (result.Succeeded)
        {
            return new ServiceResponse<string>()
            {
                StatusCode = 200,
                IsSuccess = true,
                Data = "Password Reset Successfully."
            };
        }

        _logger.LogError("Error resetting user password.");
        foreach (var error in result.Errors)
        {
            _logger.LogError(error.ToString());
        }
        return new ServiceResponse<string>()
        {
            Data = "Confirmation email not sent.",
            IsSuccess = false,
            Error = new ErrorResponse()
            {
                ErrorCode = 500,
                ErrorDescription = "Server Error while resetting user password."
            }
        };
    }
}
