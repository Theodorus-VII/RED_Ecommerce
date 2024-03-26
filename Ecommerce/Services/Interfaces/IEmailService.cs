namespace Ecommerce.Services.Interfaces;

public interface IEmailService
{
    bool SendEmail(EmailDto request);
    public void CheckEmailConfig();
}