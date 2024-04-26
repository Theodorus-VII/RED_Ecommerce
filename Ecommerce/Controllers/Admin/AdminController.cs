using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Ecommerce.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("admin")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Admin)]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IProductService _productService;
    private readonly IAuthService _authService;
    private readonly IUserAccountService _userAccountService;
    private readonly IUserManagementService _userManagementService;
    private readonly IOrderService _orderService;
    public AdminController(
        ILogger<AdminController> logger,
        IAuthService authService,
        IUserAccountService userAccountService,
        IUserManagementService userManagementService,
        IProductService productService,
        IOrderService orderService
    )
    {
        _logger = logger;
        _authService = authService;
        _userAccountService = userAccountService;
        _userManagementService = userManagementService;
        _productService = productService;
        _orderService = orderService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _authService.GetUsers());
    }

    [HttpGet("test")]
    public IActionResult Test(){
        return Ok("ADMIN CONTROLLER");
    }

    /// <summary>
    /// Admin - User Delete Endpoint
    /// </summary>
    /// <response code="200">User Deleted Successfully</response>
    /// <response code="403">Insufficient Priveledges(User tried to access this admin endpoint)</response>
    /// <response code="404">User Not Found</response>
    /// <response code="500">Server Error</response>
    /// <param name="request"></param>
    [HttpPost("admin-user-delete")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AdminUserDelete(AdminUserDeleteRequest request)
    {
        var result = await _userManagementService.AdminDeleteUser(request.Email, request.UserId);

        if (result.IsSuccess)
        {
            return Ok("User Deleted");
        }
        return StatusCode(
            result.Error.ErrorCode,
            result.Error.ErrorDescription
            );
    }

    /// <summary>
    ///     Register A New Administrator Account
    /// </summary>
    /// <remarks>
    /// Admin Registration Request:
    /// 
    ///     POST /auth/admin-register
    ///     
    ///     {   
    ///         "FirstName": "first name",
    ///         "LastName": "last name",
    ///         "Email": "somemeail@email.email",
    ///         "Password": "randompassword",
    ///         "ConfirmPassword": "randompassword",
    ///         "DefaultShippingAddress": "Garfield's house", 
    ///         "BillingAddress": "Kizaru"
    ///      }   
    /// </remarks>
    /// <response code="201">
    ///  User Created. Returns the user with the access and refresh tokens
    ///     <returns>
    ///     A User Object with access and refresh tokens
    ///     
    ///         {
    ///           "id": "563e447c-d64d-44ae-a00b-3800802e3498",
    ///           "firstName": "first name",
    ///           "lastName": "last name",
    ///           "email": "somemeail@email.email",
    ///           "defaultShippingAddress": "Garfield's house",
    ///           "billingAddress": "Kizaru",
    ///           "accessToken": "Some access token",
    ///           "refreshToken": "Some refresh token",
    ///           "phoneNumber": null
    ///         }
    ///     </returns>
    /// </response>
    /// <response code="400">Bad Request. Some field is input incorrectly</response>
    /// <response code="409">Email already in use</response>
    /// <response code="500">Other Internal Server Error</response>
    [HttpPost("admin-register")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AdminCreate(RegistrationRequest request)
    {
        _logger.LogInformation("Attempting to register a new user...");

        var response = await _authService.RegisterAdmin(request);

        if (!response.IsSuccess || response.Data is null)
        {
            return StatusCode(
                response.Error.ErrorCode,
                response.Error.ErrorDescription
            );
        }

        _logger.LogInformation("Admin Successfully Created.");

        UserDto user = response.Data;

        string baseUrl = $"{Request.Host}{Request.PathBase}";
        string action = Url.Action("ConfirmEmail", "auth")!;

        _logger.LogInformation("Sending Confirmation Email...");
        await _authService.SendConfirmationEmail(
            user: user,
            baseUrl: baseUrl,
            scheme: Request.Scheme,
            action: action
        );
        return StatusCode(statusCode: 201, user);
    }

    [HttpGet("recent_reviews")]
    public async Task<IActionResult> GetRecentRatings()
    {
        var ratings = await _productService.GetRecentProductReviews();

        var data = new {ratings=ratings};

        return Ok(
            new ApiResponse<Object>(
                true, "Ratings fetched successfully.", data
            )
        );
    }

    [HttpGet("recent_orders")]
    public async Task<IActionResult> GetRecentOrders()
    {
        var orders =  await _orderService.GetRecentOrdersAsync();
        var data = new {count=orders.Count, orders= orders};
        
        return Ok(
            new ApiResponse<Object>(
                true, "Orders fetched successfully.", data
            )
        );
    }
}