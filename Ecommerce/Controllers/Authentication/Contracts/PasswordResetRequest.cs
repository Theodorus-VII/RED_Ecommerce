using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers.Contracts;

public class PasswordResetRequest
{
    [Required(ErrorMessage = "Email Field is required")]
    [MinLength(1, ErrorMessage = "Email needs to be more than 1 character")]
    public string Email { get; set; }
    public string ResetToken { get; set; }

    [Required(ErrorMessage = "Password Field is required")]
    [MinLength(7, ErrorMessage = "Password needs to be more than 7 character")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password Field is required.")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
    public string ConfirmPassword { get; set; }

    public PasswordResetRequest(
        string email,
        string resetToken,
        string password,
        string confirmPassword
    )
    {
        Email = email;
        ResetToken = resetToken;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
}
