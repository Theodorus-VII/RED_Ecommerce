namespace Ecommerce.Services.Interfaces;

public interface IEmailService
{
    void SendEmail(EmailDto request);
    public void checkEmailConfig();
}