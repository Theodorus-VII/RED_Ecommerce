using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services;

public class UserAccountService : IUserAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserAccountService> _logger;

    public UserAccountService(UserManager<User> userManager, ILogger<UserAccountService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> UpdateUserDetails(Guid userId, UserPatchRequest request)
    {
        try
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            user.Email = request.Email ?? user.Email;
            user.UserName = request.Email ?? user.UserName;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.BillingAddress = request.BillingAddress ?? user.BillingAddress;
            user.DefaultShippingAddress =
                request.DefaultShippingAddress ?? user.DefaultShippingAddress;

            if (request.NewPassword != null && request.OldPassword != null)
            {
                await _userManager.ChangePasswordAsync(
                    user,
                    request.OldPassword,
                    request.NewPassword
                );
            }
            else if (request.NewPassword != null && request.OldPassword == null)
            {
                throw new Exception("Old password not provided");
            }

            await _userManager.UpdateAsync(user);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"{e}");
            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        try
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            await _userManager.DeleteAsync(user);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"{e}");
            return false;
        }
    }
}
