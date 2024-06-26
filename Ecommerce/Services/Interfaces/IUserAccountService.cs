using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Utilities;

namespace Ecommerce.Services.Interfaces;

public interface IUserAccountService{
    public Task<User?> GetUserById(Guid UserId);

    public Task<User?> GetUserByEmail(string Email);

    public Task<string> GetUserRole(User user);
    public Task<IEnumerable<UserDto>> GetAllUsers();
    public Task<IEnumerable<UserDto>> GetUsersByRole(string role = "Customer");

}