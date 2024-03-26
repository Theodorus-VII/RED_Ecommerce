using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services;

public interface IAuthService
{
    public IEnumerable<User> GetUsers();
    public Task<IAuthResponse> RegisterUser(RegistrationRequest request);
    public Task<IAuthResponse> LoginUser(LoginRequest request);
}