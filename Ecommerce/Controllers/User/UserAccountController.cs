using System.Security.Claims;
using Ecommerce.Controllers.Contracts;
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

    [HttpDelete()]
    public IActionResult DeleteUser()
    {
        return Ok("sth");
    }
}
