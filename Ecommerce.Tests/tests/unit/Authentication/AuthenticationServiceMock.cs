using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Utilities;

namespace Ecommerce.Tests.Unit.Authentication;

public class AuthServiceMock : IAuthService
{
    private List<User> _users;

    public AuthServiceMock()
    {
        _users = new List<User>()
        {
            new("email@email.com", "firstname", "lastname")
                {
                    PasswordHash = "user1 password hash"
                },
            new("anotheremail@email.com", "testuser2", "doesntexisteither")
                {
                    PasswordHash = "user 2 password hash"
                },
            new("yetanotheremail@email.com", "testuser3", "doesntexisteithertoo")
                {
                    PasswordHash = "user 3 password hash"
                },
        };
    }

    private User? FindUserByEmailHelper(string email)
    {
        foreach (var user in _users)
        {
            if (user.Email == email)
            {
                return user;
            }
        }
        return null;
    }
    private User? FindUserByIdHelper(string userId)
    {
        foreach (var user in _users)
        {
            if (user.Id.ToString() == userId)
            {
                return user;
            }
        }
        return null;
    }

    public Task<IServiceResponse<bool>> ConfirmEmail(string email, string token)
    {
        var user = FindUserByEmailHelper(email);
        if (user != null)
        {
            if (user.Email == email)
            {
                IServiceResponse<bool> successResponse = new ServiceResponse<bool>()
                {
                    Data = true,
                    IsSuccess = true
                };

                return new Task<IServiceResponse<bool>>(() => successResponse);
            }
        }
        IServiceResponse<bool> failResponse = new ServiceResponse<bool>()
        {
            Data = false,
            IsSuccess = false,
            Error = new ErrorResponse()
            {
                ErrorCode = 404,
                ErrorDescription = "User Not Found"
            }
        };

        return new Task<IServiceResponse<bool>>(() => failResponse);
    }

    public Task<IServiceResponse<string>> GenerateEmailConfirmationToken(string email)
    {
        IServiceResponse<string> failResponse;
        IServiceResponse<string> successResponse;

        var user = FindUserByEmailHelper(email);
        if (user != null)
        {
            if (user.Email == email)
            {
                if (user.EmailConfirmed)
                {
                    failResponse = new ServiceResponse<string>()
                    {
                        IsSuccess = false,
                        Error = new ErrorResponse()
                        {
                            ErrorCode = 404,
                            ErrorDescription = "User Account Already Confirmed"
                        }
                    };
                    return new Task<IServiceResponse<string>>(() => failResponse);
                }
                successResponse = new ServiceResponse<string>()
                {
                    IsSuccess = true,
                    Data = "Email Confirmation Token"
                };
                return new Task<IServiceResponse<string>>(() => successResponse);
            }
        }
        failResponse = new ServiceResponse<string>()
        {
            IsSuccess = false,
            Error = new ErrorResponse()
            {
                ErrorCode = 404,
                ErrorDescription = "User Not Found"
            }
        };
        return new Task<IServiceResponse<string>>(() => failResponse);
    }

    public IEnumerable<User> GetUsers()
    {
        return _users.ToList();
    }

    public Task<IServiceResponse<UserDto>> LoginUser(LoginRequest request)
    {
        IServiceResponse<UserDto> failResponse;
        IServiceResponse<UserDto> successResponse;

        foreach (var user in _users)
        {
            if (user.Email == request.Email)
            {
                if (user.PasswordHash == request.Password)
                {
                    successResponse = new ServiceResponse<UserDto>()
                    {
                        Data = new UserDto(user),
                        IsSuccess = true
                    };
                    return new Task<IServiceResponse<UserDto>>(() => successResponse);
                }
                failResponse = new ServiceResponse<UserDto>()
                {
                    IsSuccess = false,
                    Error = new ErrorResponse()
                    {
                        ErrorCode = 401,
                        ErrorDescription = "Invalid Password"
                    }
                };
                return new Task<IServiceResponse<UserDto>>(() => failResponse);
            }
        }
        failResponse = new ServiceResponse<UserDto>()
        {
            IsSuccess = false,
            Error = new ErrorResponse()
            {
                ErrorCode = 404,
                ErrorDescription = "User not found"
            }
        };
        return new Task<IServiceResponse<UserDto>>(() => failResponse);
    }

    public Task<IServiceResponse<bool>> LogoutUser(string userId)
    {
        IServiceResponse<bool> failResponse;
        IServiceResponse<bool> successResponse;

        var user = FindUserByIdHelper(userId);
        if (user == null)
        {
            failResponse = new ServiceResponse<bool>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorDescription = "User not found"
                }
            };
            return new Task<IServiceResponse<bool>>(() => failResponse);
        }

        successResponse = new ServiceResponse<bool>()
        {
            IsSuccess = true,
            Data = true
        };
        return new Task<IServiceResponse<bool>>(() => successResponse);
    }

    public Task<IServiceResponse<UserDto>> RefreshToken(string expiredToken, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<IServiceResponse<UserDto>> RegisterAdmin(RegistrationRequest request)
    {
        return RegisterUser(request, "Customer");
    }

    public Task<IServiceResponse<UserDto>> RegisterUser(RegistrationRequest request, string role)
    {
        IServiceResponse<UserDto> failResponse;
        IServiceResponse<UserDto> successResponse;


        var user_exists = FindUserByEmailHelper(request.Email) != null;

        if (user_exists)
        {
            failResponse = new ServiceResponse<UserDto>()
            {
                IsSuccess = false,
                Error = new ErrorResponse()
                {
                    ErrorCode = 409,
                    ErrorDescription = "Email already in use"
                }
            };
        }
        var user = new User(
                request.Email,
                request.FirstName,
                request.LastName,
                request.DefaultShippingAddress,
                request.BillingAddress);
        _users.Add(user);
        successResponse = new ServiceResponse<UserDto>()
        {
            Data = new UserDto(user, "access token", "refresh token"),
            IsSuccess = true,
            StatusCode = 201
        };
        return new Task<IServiceResponse<UserDto>>(() => successResponse);
    }

    public Task<IServiceResponse<UserDto>> RegisterCustomer(RegistrationRequest request)
    {
        return RegisterUser(request, "Administrator");
    }

    public Task<IServiceResponse<string>> ResetPassword(string email, string resetToken, string newPassword)
    {
        throw new NotImplementedException();
    }

    public Task<IServiceResponse<bool>> SendConfirmationEmail(UserDto user, string baseUrl, string scheme, string action)
    {
        throw new NotImplementedException();
    }

    public Task<IServiceResponse<string>> SendPasswordResetEmail(User user, string baseUrl, string scheme, string action)
    {
        throw new NotImplementedException();
    }
}
