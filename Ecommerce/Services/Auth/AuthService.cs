using System.Security.Claims;
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
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IJwtTokenGenerator tokenGenerator,
        ILogger<AuthService> logger
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public IEnumerable<User> GetUsers()
    {
        return _userManager.Users;
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

            // ClaimsIdentity identity = new(IdentityConstants.ApplicationScheme);
            // identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            // identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            // identity.AddClaim(new Claim(ClaimTypes.Role, "Customer"));

            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                _logger.LogInformation($"User Role: {role}");
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            

            string token = _tokenGenerator.GenerateToken(user, claims);

            return new AuthSucessResponse(new UserDto(user, token));
        }
        catch
        {
            _logger.LogError("Invalid Username or Password");
            return new AuthFailResponse(error: "Invalid Username or Password");
        }
    }

    public async Task<IAuthResponse> RegisterUser(RegistrationRequest request)
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
            var assignRoleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
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
            return new AuthSucessResponse(new UserDto(user, token));
        }
        catch (Exception e)
        {
            _logger.LogError($"Registration Error: {e}");
            return new AuthFailResponse(e.Message);
        }
    }
}
