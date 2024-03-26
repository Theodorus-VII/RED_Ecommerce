using Ecommerce.Controllers.Contracts;

namespace Ecommerce.Services.Interfaces;

public interface IUserAccountService{
    public Task<bool> UpdateUserDetails(Guid userId, UserPatchRequest request);
}