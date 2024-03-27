using System.Security.Claims;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;
public interface IJwtTokenGenerator{
  public string GenerateToken(User user, IEnumerable<Claim> claims);
  public string GenerateRefreshToken();
public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}