using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Utilities;

namespace Ecommerce.Services.Interfaces;

public interface IUserManagementService
{
    public Task<IServiceResponse<bool>> UpdateUserDetails(Guid userId, UserPatchRequest request);
    public Task<IServiceResponse<bool>> DeleteUser(Guid userId);
}