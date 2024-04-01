using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserAccountService _userAccountService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IEmailService emailService,
        IUserAccountService userAccountService,
        ILogger<AuthController> logger
    )
    {
        _authService = authService;
        _emailService = emailService;
        _userAccountService = userAccountService;
        _logger = logger;
    }

    //test route
    [HttpGet("test")]
    public IActionResult GetUsers()
    {
        return Ok(_authService.GetUsers());
    }

    //registration endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
    {
        _logger.LogInformation("Attempting to register a new user...");

        IAuthResponse response = await _authService.RegisterCustomer(registrationRequest);

        if (response.IsAuthSuccess())
        {
            _logger.LogInformation("User Successfully Created.");
            UserDto user = response.User;
            string baseUrl = $"{Request.Host}{Request.PathBase}";
            string action = Url.Action("ConfirmEmail", "auth")!;
            if (
                !await _authService.SendConfirmationEmail(
                    user: user,
                    baseUrl: baseUrl,
                    scheme: Request.Scheme,
                    action: action
                )
            )
            {
                _logger.LogInformation("Unable to send confirmation email");
            }
            else
            {
                _logger.LogInformation("Confirmation email sent");
            }
            return Ok(user);
        }
        return StatusCode(500, response.Error.ToString());
    }

    //login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        _logger.LogInformation("Attempting to login user...");

        var response = await _authService.LoginUser(loginRequest);

        if (!response.IsAuthSuccess())
        {
            return Unauthorized(response.Error);
        }

        return Ok(response.User);
    }

    [HttpPost("login-delete")]
    public async Task<IActionResult> LoginDelete(LoginRequest loginRequest)
    {
        IAuthResponse loginResponse = await _authService.LoginUser(loginRequest);
        var deleteResponse = await _userAccountService.DeleteUser(loginResponse.User.Id);
        return Ok(deleteResponse);
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LogOut()
    {
        var userId = ExtractUser.GetUserId(HttpContext);

        if (userId is null)
        {
            return BadRequest();
        }

        _logger.LogInformation("Attempting to logout user...");
        await _authService.LogoutUser(userId.Value.ToString());
        _logger.LogInformation("User logged out");
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenRequestModel request)
    {
        if (request.RefreshToken == "" || request.AccessToken == "")
        {
            return BadRequest("Refresh and Access token have to be provided.");
        }
        var response = await _authService.RefreshToken(
            expiredToken: request.AccessToken,
            refreshToken: request.RefreshToken
        );

        if (response.IsAuthSuccess())
        {
            return Ok(response.User);
        }
        return Unauthorized(response.Error);
    }

    [HttpPost("admin-register")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AdminCreate(RegistrationRequest request)
    {
        _logger.LogInformation("Attempting to register a new user...");

        var response = await _authService.RegisterAdmin(request);

        if (response.IsAuthSuccess())
        {
            _logger.LogInformation("Admin Successfully Created.");

            var user = response.User;
            var confirmationEmail = new EmailDto
            {
                Recipient = user.Email,
                Subject = "Welcome to _______ Commerce",
                Message = $@"<p>Your new admin account at _______ Commerce has been created.</p>"
            };
            _logger.LogInformation("Sending Confirmation Email...");
            await _emailService.SendEmail(confirmationEmail);
            return Ok(user);
        }
        return BadRequest(response.Error.ToString());
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var user = await _userAccountService.GetUserById(userId);

        if (user is null)
        {
            return BadRequest("User not found.");
        }

        _logger.LogInformation(user.Email);

        var result = await _authService.ConfirmEmail(user.Email, token);
        _logger.LogDebug(result.ToString());
        if (result)
        {
            user = await _userAccountService.GetUserById(userId);
            if (user != null && user.EmailConfirmed)
            {
                return Ok("Email Confirmed");
            }
        }
        return StatusCode(500, "Error Confirming Account, please try again later.");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _userAccountService.GetUserByEmail(email);
        if (user is null)
        {
            return BadRequest("User not found");
        }

        string baseUrl = $"{Request.Host}{Request.PathBase}";
        string action = Url.Action("ResetPassword", "auth")!;

        var result = await _authService.SendPasswordResetEmail(
            user: user,
            baseUrl: baseUrl,
            action: action,
            scheme: Request.Scheme
        );

        if (result)
        {
            return Ok("Password Reset Email sent");
        }

        return StatusCode(
            500,
            "Server Error: error sending password reset email. Try again later."
        );
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(PasswordResetRequest passwordResetRequest)
    {
        if (
            await _authService.ResetPassword(
                passwordResetRequest.Email,
                passwordResetRequest.ResetToken,
                passwordResetRequest.Password
            )
        )
        {
            return Ok("Password has been reset successfully. Please log in with the new password.");
        }
        return StatusCode(500, "Server Error: Error resetting user password");
    }
}
