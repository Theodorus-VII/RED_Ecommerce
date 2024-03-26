using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _logger = logger;
        _authService = authService;
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

        var response = await _authService.RegisterUser(registrationRequest);

        if (response.IsAuthSuccess())
        {
            var user = response.User;
            return Ok(user);
        }
        return BadRequest(response.Error.ToString());
    }

    //login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        _logger.LogInformation("Attempting to login user...");

        var response = await _authService.LoginUser(loginRequest);

        if (!response.IsAuthSuccess())
        {
            return BadRequest(response.Error);
        }

        return Ok(response.User);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenRequestModel request)
    {
        if (request is null)
        {
            return BadRequest("Invalid client request");
        }

        var response = await _authService.RefreshToken(
            expiredToken: request.AccessToken,
            refreshToken: request.RefreshToken
        );

        return Ok(response.User);
    }
}
