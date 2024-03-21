using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Services;

public class TestService
{
    private readonly ApplicationDbContext _applicationDbContext;

    // inject the dbContext into the services like this
    public TestService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public string ServiceTest()
    {
        return "Just some string from the test service";
    }

    public IEnumerable<User> DbServiceTest()
    {
        return _applicationDbContext.Users;
    }

}