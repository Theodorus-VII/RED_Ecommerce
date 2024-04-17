using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserAccountService> _logger;
    private readonly IUserAccountService _userAccountService;

    public UserManagementService(
        UserManager<User> userManager,
        ILogger<UserAccountService> logger,
        IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IServiceResponse<bool>> UpdateUserDetails(Guid userId, UserPatchRequest request)
    {
        try
        {
            var user = await _userAccountService.GetUserById(userId);

            if (user == null)
            {
                return new ServiceResponse<bool>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 404,
                        ErrorDescription = "User Not Found"
                    }
                };
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
                return new ServiceResponse<bool>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 400,
                        ErrorDescription = "Old Password Not Provided"
                    }
                };
            }

            await _userManager.UpdateAsync(user);
            return new ServiceResponse<bool>()
            {
                IsSuccess = true,
                Data = true
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"{e}");
            return new ServiceResponse<bool>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Internal Server Error"
                }
            };
        }
    }

    public async Task<IServiceResponse<bool>> DeleteUser(Guid userId)
    {
        try
        {
            var user = await _userAccountService.GetUserById(userId);

            if (user == null)
            {
                return new ServiceResponse<bool>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 404,
                        ErrorDescription = "User Not Found"
                    }
                };
            }

            await _userManager.DeleteAsync(user);
            return new ServiceResponse<bool>()
            {
                IsSuccess = true,
                Data = true
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"{e}");
            return new ServiceResponse<bool>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorDescription = "Internal Server Error"
                }
            };
        }
    }

    public async Task<IServiceResponse<UserDto>> GetUserDetails(Guid userId)
    {
        var user = await _userAccountService.GetUserById(userId);

        if (user is null)
        {
            return new ServiceResponse<UserDto>()
            {
                IsSuccess = false,
                StatusCode = 404,
                Error = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorDescription = "User Not Found"
                }
            };
        }

        var role = await _userAccountService.GetUserRole(user);

        return new ServiceResponse<UserDto>()
        {
            IsSuccess = true,
            StatusCode = 200,
            Data = new UserDto(user: user, role: role)
        };
    }
}
