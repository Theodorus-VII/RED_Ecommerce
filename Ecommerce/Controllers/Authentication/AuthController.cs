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
            return Ok($"{response}");
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

        var user = response.User;

        // ClaimsIdentity identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        // identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        // identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));

        // await HttpContext.SignInAsync(
        //     IdentityConstants.ApplicationScheme,
        //     new ClaimsPrincipal(identity));

        return Ok(user);
    }
}