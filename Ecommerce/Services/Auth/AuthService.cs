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

    public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
    {
        return _tokenGenerator.GetPrincipalFromExpiredToken(token);
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

            return new AuthSucessResponse(new UserDto(user, token, refreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Login Error: {e}");
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
            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthSucessResponse(new UserDto(user, token, refreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Registration Error: {e}");
            return new AuthFailResponse(e.Message);
        }
    }

    public async Task<IAuthResponse> RefreshToken(string expiredToken, string refreshToken)
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
                throw new Exception("Invalid User. Claim corresponding to Id not found");
            }

            Guid userId = Guid.Parse(userIdClaim.Value);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            _logger.LogInformation($"{user}");

            if (user is null)
            {
                throw new Exception("Invalid user");
            }
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                _logger.LogInformation($"User refresh token: {user.RefreshToken}");
                _logger.LogInformation($"Supplied refresh token: {refreshToken}");
                _logger.LogInformation($"Token Expiry: {user.RefreshTokenExpiry}");


                throw new Exception("Invalid or Expired Refresh Token");
            }
            var newAccessToken = _tokenGenerator.GenerateToken(user, principal.Claims);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthSucessResponse(new UserDto(user, newAccessToken, newRefreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Refresh Token Error: {e}");
            return new AuthFailResponse("Invalid Client Request");
        }
    }
}
