using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Utilities;

namespace Ecommerce.Services.Interfaces;

public interface IUserAccountService{
    public Task<User?> GetUserById(Guid UserId);

    public Task<User?> GetUserByEmail(string Email);
}