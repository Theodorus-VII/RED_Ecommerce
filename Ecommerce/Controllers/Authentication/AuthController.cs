using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IEmailService _emailService;

    public AuthController(
        IAuthService authService,
        IEmailService emailService,
        ILogger<AuthController> logger
    )
    {
        _authService = authService;
        _emailService = emailService;
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

        var response = await _authService.RegisterCustomer(registrationRequest);

        if (response.IsAuthSuccess())
        {
            _logger.LogInformation("User Successfully Created.");

            var user = response.User;
            var confirmationEmail = new EmailDto
            {
                Recipient = user.Email,
                Subject = "Welcome to _______ Commerce",
                Message = $@"<p>Your new account at _______ Commerce has been created.</p>"
            };
            _logger.LogInformation("Sending Confirmation Email...");
            await _emailService.SendEmail(confirmationEmail);
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

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LogOut()
    {
        if (HttpContext.Items.TryGetValue("UserId", out var userIdObj))
        {
            var userId = userIdObj.ToString();
            _logger.LogInformation(userId);
        }
        _logger.LogInformation("Attempting to logout user...");

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
}
