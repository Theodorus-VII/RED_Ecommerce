using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserAccountService{
    public Task<User?> GetUserById(Guid UserId);

    public Task<User?> GetUserByEmail(string Email);
    public Task<bool> UpdateUserDetails(Guid userId, UserPatchRequest request);
    public Task<bool> DeleteUser(Guid userId);
}