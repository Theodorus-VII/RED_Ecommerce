using Ecommerce.Models;

namespace Ecommerce.Controllers.Contracts;

public interface IAuthResponse
{
    public bool IsAuthSuccess();
    public string Error { get; set; }
    public UserDto User { set; }
}