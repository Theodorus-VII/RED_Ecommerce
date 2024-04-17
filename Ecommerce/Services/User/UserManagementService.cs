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
        var user = await _userAccountService.GetUserById(userId);

        if (user == null)
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }

        user = user.UpdateUser(
            request.Email,
            request.FirstName,
            request.LastName,
            request.DefaultShippingAddress,
            request.BillingAddress
        );
        // user provided both new and old password. attempt to update the password.
        if (request.NewPassword != null && request.OldPassword != null)
        {
            var result = await _userManager.ChangePasswordAsync(
                user,
                request.OldPassword,
                request.NewPassword
            );

            if (!result.Succeeded)
            {
                _logger.LogError("Error Changing Password: {}", result.Errors.ToList());
                return ServiceResponse<bool>.FailResponse(
                    statusCode: StatusCodes.Status400BadRequest,
                    errorDescription: $"Error Changing Password: {result.Errors.ToList()}"
                );
            }
        }
        // User provided a new password, but not the old password.
        else if (request.NewPassword != null && request.OldPassword == null)
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorDescription: "Old Password Not Provided"
            );
        }

        await _userManager.UpdateAsync(user);

        return ServiceResponse<bool>.SuccessResponse(
            statusCode: StatusCodes.Status200OK,
            data: true
        );
    }

    public async Task<IServiceResponse<bool>> DeleteUser(Guid userId)
    {

        var user = await _userAccountService.GetUserById(userId);

        if (user == null)
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }

        await _userManager.DeleteAsync(user);

        return ServiceResponse<bool>.SuccessResponse(
            data: true,
            statusCode: StatusCodes.Status200OK
        );
    }

    public async Task<IServiceResponse<bool>> AdminDeleteUser(
        string? email = null,
        string? userId = null)
    {
        User? user = null;
        if (userId != null)
        {
            user = await _userAccountService.GetUserById(Guid.Parse(userId));
        }
        else if (email != null)
        {
            user = await _userAccountService.GetUserByEmail(email);
        } 
        else 
        {
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorDescription: "Please provide the email or userID of the user to delete."
            );
        }
        
        if (user == null)
        {
            return ServiceResponse<bool>.FailResponse(
              statusCode: StatusCodes.Status404NotFound,
              errorDescription: "User Not Fount"  
            );
        }

        return await DeleteUser(user.Id);
    }

    public async Task<IServiceResponse<UserDto>> GetUserDetails(Guid userId)
    {
        var user = await _userAccountService.GetUserById(userId);

        if (user is null)
        {
            return ServiceResponse<UserDto>.FailResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorDescription: "User Not Found"
            );
        }

        var role = await _userAccountService.GetUserRole(user);

        return ServiceResponse<UserDto>.SuccessResponse(
            statusCode: StatusCodes.Status200OK,
            data: new UserDto(user: user, role: role)
        );
    }
}
