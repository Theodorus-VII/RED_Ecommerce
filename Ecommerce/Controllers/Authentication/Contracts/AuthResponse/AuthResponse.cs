using Ecommerce.Models;

namespace Ecommerce.Controllers.Contracts;

public class AuthSucessResponse : IAuthResponse
{
    public string Error { get; set; }
    public UserDto User { get; set; }

    public AuthSucessResponse(UserDto user)
    {
        User = user;
        Error = "";
    }

    public bool IsAuthSuccess() => true;
}

public class AuthFailResponse : IAuthResponse
{
    public string Error { get; set; }
    public UserDto? User { get; set; } = null;

    public bool IsAuthSuccess() => false;

    public AuthFailResponse(string error)
    {
        Error = error;
    }
}