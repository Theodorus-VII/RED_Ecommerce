using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Controllers;
[ApiController]
[Route("test")]
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
        return Ok("Ok from the test controller");
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
}