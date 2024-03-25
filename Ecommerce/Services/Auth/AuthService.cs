using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Inerfaces;
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

            ClaimsIdentity identity = new(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));

            string token = _tokenGenerator.GenerateToken(user);
            
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                Console.WriteLine($"Role: {role}");
                _logger.LogInformation($"Role: {role}");
            }
            
            return new AuthSucessResponse(new UserDto(user, token));
        }
        catch
        {
            Console.WriteLine("Invalid Username or Password");
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
            Console.WriteLine("Creating user...");
            User user =
                new(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.DefaultShippingAddress,
                    request.BillingAddress
                );
            Console.WriteLine(request.Password);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                Console.WriteLine($"Encountered Identity Errors: {result.Errors}");
                throw new Exception("Server Error");
            }
            Console.WriteLine("User added to server.");
            var assignRoleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!assignRoleResult.Succeeded)
            {
                Console.WriteLine($"Encountered Role Assignment Error: {assignRoleResult.Errors}");
            }
            string token = _tokenGenerator.GenerateToken(user);
            return new AuthSucessResponse(new UserDto(user, token));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Registration Error: {e}");
            return new AuthFailResponse(e.Message);
        }
    }
}
