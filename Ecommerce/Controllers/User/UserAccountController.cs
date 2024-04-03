using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
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

    [HttpGet("test")]
    public IActionResult Test()
    {
        var userId = ExtractUser.GetUserId(HttpContext);
        if (userId is null)
        {
            return BadRequest("Invalid token");
        }
        _logger.LogInformation(userId.Value.ToString());
        return Ok();
    }

    [HttpGet()]
    public async Task<IActionResult> GetUserDetails()
    {
        var userId = ExtractUser.GetUserId(HttpContext);

        _logger.LogInformation(userId.ToString());

        if (userId is null)
        {
            return BadRequest("Invalid token");
        }

        var user = await _userAccountService.GetUserById(userId.Value);
        if (user is null)
        {
            return NotFound("User not found");
        }
        
        return Ok(new UserDto(user));
    }

    [HttpPatch("update")]
    public async Task<IActionResult> UpdateUserDetails(UserPatchRequest request)
    {
        var userId = ExtractUser.GetUserId(HttpContext);
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _userAccountService.UpdateUserDetails(userId.Value, request);
        if (result)
        {
            return Ok("User Updated Successfully");
        }
        return BadRequest();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = ExtractUser.GetUserId(HttpContext);

        if (userId is null)
        {
            return Unauthorized("Invalid Token");
        }
        // extract the user Id from the claim.
        var result = await _userAccountService.DeleteUser(userId.Value);
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

        if (user is null)
        {
            return BadRequest("User not found");
        }
        var userId = user.Id;
        var result = await _userAccountService.DeleteUser(userId);

        if (result)
        {
            return Ok("User Deleted");
        }
        return BadRequest("Server Error");
    }

    
}
