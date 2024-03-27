using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Configuration;

[ApiController]
[Route("user")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserAccountController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly ILogger<UserAccountController> _logger;

    public UserAccountController(
        IUserAccountService userAccountService,
        ILogger<UserAccountController> logger
    )
    {
        _userAccountService = userAccountService;
        _logger = logger;
    }

    [HttpPatch("update")]
    public async Task<IActionResult> UpdateUserDetails(UserPatchRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("User Id claim not found within the token provided");
            return BadRequest("Invalid user");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);

        var result = await _userAccountService.UpdateUserDetails(userId, request);
        if (result)
        {
            return Ok("User Updated Successfully");
        }
        return BadRequest();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("User Id claim not found within the token provided");
            return BadRequest("Invalid user");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);

        var result = await _userAccountService.DeleteUser(userId);
        if (result)
        {
            return Ok("User Account Deleted");
        }
        return BadRequest("Server Error");
    }
    

    [HttpPost("admin-user-delete")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AdminUserDelete(AdminUserDeleteRequest request)
    {
        User? user;
        if (request.UserId != null)
        {
            user = await _userAccountService.GetUserById(Guid.Parse(request.UserId));
        }
        else if (request.Email != null)
        {
            user = await _userAccountService.GetUserByEmail(request.Email);
        }
        else
        {
            return BadRequest("Please provide the User Id or email of the user to delete");
        }

        if (user is null){
            return BadRequest("User not found");
        }
        var userId = user.Id;
        var result = await _userAccountService.DeleteUser(userId);

        if (result){
            return Ok("User Deleted");
        }
        return BadRequest("Server Error");
    }
}
