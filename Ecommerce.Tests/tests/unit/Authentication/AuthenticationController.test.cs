using Ecommerce.Controllers;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.Tests.Unit.Authentication.Controller;

public class AuthenticationControllerTest
{
    private readonly AuthController _authController;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserAccountService> _mockUserAccountService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;

    public AuthenticationControllerTest()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockUserAccountService = new Mock<IUserAccountService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        
        
        _authController = new AuthController(
            _mockAuthService.Object, 
            _mockUserAccountService.Object, 
            _mockLogger.Object);
    }

    [Fact]
    public void SampleTest()
    {
        //arrange
        _mockAuthService.Setup(
            service => service.GetUsers()
        ).Returns(new List<User>()
        {
            new User("email@email.com", "firstname", "lastname"),
            new User("email2@email.com", "firstname2", "lastnam2"),
            new User("email3@email.com", "firstname3", "lastnam3")
        });

        //act
        var result = _authController.GetUsers();

        //assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<User>>(okResult.Value);
        Assert.Equal(3, list.Count);
        Assert.Contains(
            list, 
            user => user.Email == "email@email.com" && user.FirstName == "firstname");
        Assert.DoesNotContain(
            list, user => user.Email == "email@email.com" && user.FirstName == "firstname2");
    }

}