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
    private readonly IEmailService _emailService;

    public TestController(TestService testService, IEmailService emailService)
    {
        _emailService = emailService;
        _testService = testService;
    }

    [HttpGet()]
    public IActionResult GetTestResult()
    {
        _emailService.CheckEmailConfig();
        return Ok("Ok from the test controller");
    }


    [HttpPost("sendmail")]
    public IActionResult SendMail()
    {
        var email = new EmailDto();
        email.Recipient = "kari.fay98@ethereal.email";
        email.Message = "IT WORKS!!!!!!!";
        email.Subject = "Email test";
        _emailService.SendEmail(
            email
        );
        return Ok("Email sent");
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
