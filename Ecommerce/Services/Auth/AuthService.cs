using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Inerfaces;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        UserManager<User> userManager,
        IJwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
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

            ClaimsIdentity identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));

            string token = _tokenGenerator.GenerateToken(user);
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
            Console.WriteLine("Creating user...");
            User user = new(
                request.Email,
                request.FirstName,
                request.LastName,
                request.DefaultShippingAddress,
                request.BillingAddress
            );
            Console.WriteLine(request.Password);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                Console.WriteLine("User added to server.");
                await _userManager.AddToRoleAsync(user, "Customer");
                string token = _tokenGenerator.GenerateToken(user);
                return new AuthSucessResponse(new UserDto(user, token));
            }
            throw new Exception($"Encountered Identity Errors: {result.Errors}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Registration Error: {e}");
            // IdentityError error = new()
            // {
            //     Code = "500",
            //     Description = "Server Error"
            // };
            // IdentityResult result = IdentityResult.Failed(error);
            return new AuthFailResponse("Server Error");
        }
    }
}
