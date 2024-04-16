using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Tests.Unit.Authentication;

public class UserAccountServiceMock : IUserAccountService
{
    public Task<User?> GetUserByEmail(string Email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserById(Guid UserId)
    {
        throw new NotImplementedException();
    }
}
