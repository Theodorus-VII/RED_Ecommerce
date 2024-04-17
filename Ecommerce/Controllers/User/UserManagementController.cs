using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.Configuration;

[ApiController]
[Route("user")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserManagementController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly IUserManagementService _userManagementService;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(
        IUserManagementService userManagementService,
        IUserAccountService userAccountService,
        ILogger<UserManagementController> logger
    )
    {
        _userAccountService = userAccountService;
        _userManagementService = userManagementService;
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


    /// <summary>
    /// Get user details
    /// </summary>
    /// <remarks>
    /// Retrieves the details of the user identified by the token.
    /// </remarks>
    /// <response code="401">Invalid Token</response>
    /// <response code="404">User Not Found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet()]
    [SwaggerResponse(200, "User Details retrieved successfully", typeof(UserDto))]
    public async Task<IActionResult> GetUserDetails()
    {
        Guid? userId = ExtractUser.GetUserId(HttpContext);

        _logger.LogInformation(userId.ToString());

        if (userId is null)
        {
            return StatusCode(
                401,
                "Invalid token"
            );
        }
        var response = await _userManagementService.GetUserDetails(userId.Value);
        return StatusCode(
            statusCode: response.StatusCode,
            value: response.Data);
    }


    /// <summary>
    /// Update User Details
    /// </summary>
    /// <remarks>
    ///      User Patch Request
    /// </remarks>
    /// <response code="200">Successfully Updated</response>
    /// <response code="401">Invalid Token</response>
    /// <response code="500">Internal Server Error</response>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateUserDetails(UserPatchRequest request)
    {
        var userId = ExtractUser.GetUserId(HttpContext);
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _userManagementService.UpdateUserDetails(userId.Value, request);
        if (result.IsSuccess)
        {
            return Ok("User Updated Successfully");
        }
        return StatusCode(
            statusCode: result.Error.ErrorCode,
            value: result.Error.ErrorDescription
        );
    }



    /// <summary>
    /// Delete User Account
    /// </summary>
    /// <response code="200">User Account deleted</response>
    /// <response code="401">Invalid token</response>
    /// <response code="404">User Not Found</response>
    /// <response code="500">Internal Server Error</response>
    /// <returns></returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = ExtractUser.GetUserId(HttpContext);

        if (userId is null)
        {
            return Unauthorized("Invalid Token");
        }
        // extract the user Id from the claim.
        var result = await _userManagementService.DeleteUser(userId.Value);
        if (result.IsSuccess)
        {
            return Ok("User Account Deleted");
        }
        return StatusCode(
            statusCode: result.Error.ErrorCode,
            value: result.Error.ErrorDescription
        );
    }
}
