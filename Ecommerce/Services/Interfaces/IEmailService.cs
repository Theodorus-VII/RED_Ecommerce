namespace Ecommerce.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmail(EmailDto request);
    public void CheckEmailConfig();
}