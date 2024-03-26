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

    public async Task<IEnumerable<Product>> DbServiceTest()
    {
        if (!_applicationDbContext.Products.Any())
        {
            var p = new Product();
            await _applicationDbContext.Products.AddAsync(p);
            _applicationDbContext.SaveChanges();
        }
        return _applicationDbContext.Products;
    }

}