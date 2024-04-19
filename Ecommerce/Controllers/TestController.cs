using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Route("test")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Customer)]
public class TestController : ControllerBase
{
    private readonly TestService _testService;

    public TestController(TestService testService)
    {
        _testService = testService;
    }

    [HttpGet()]
    public IActionResult GetTestResult()
    {
        
        return Redirect("red://ecommerce.com/cart");
        // return Ok("Ok from the test controller");
    }

    [HttpGet("service_test")]
    public IActionResult GetServiceTestResult()
    {
        var serviceResult = _testService.ServiceTest();
        return Ok(serviceResult);
    }

    [HttpGet("service_db_test")]
    public IActionResult GetDbServiceTest()
    {
        var serviceResult = _testService.DbServiceTest();
        return Ok(serviceResult);
    }

    [HttpGet("authTest")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult AuthenticationTest()
    {
        return Ok("Authenticated");
    }

    [HttpGet("redirect")]
    public IActionResult RedirectTest(string path)
    {
        Console.WriteLine($"RedirectUrl: red://{path}");
        return Redirect($"red://{path}");
    }
}
