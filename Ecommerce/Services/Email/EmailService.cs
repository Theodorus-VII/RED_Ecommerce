using Ecommerce.Configuration;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Ecommerce.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfiguration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailConfiguration emailConfiguration, ILogger<EmailService> logger)
    {
        _emailConfiguration = emailConfiguration;
        _logger = logger;
    }

    public void CheckEmailConfig()
    {
        Console.WriteLine(_emailConfiguration.SmtpServer);
        return;
    }

    public async Task<IServiceResponse<bool>> SendEmail(EmailDto request)
    {
        try
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_emailConfiguration.From));
            email.To.Add(MailboxAddress.Parse(request.Recipient));
            email.Subject = request.Subject;

            email.Body = new TextPart(TextFormat.Html) { Text = request.Message };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _emailConfiguration.SmtpServer,
                _emailConfiguration.Port,
                SecureSocketOptions.StartTls
            );
            smtp.Authenticate(_emailConfiguration.From, _emailConfiguration.Password);
            await smtp.SendAsync(email);
            _logger.LogInformation("Email sent");
            smtp.Disconnect(true);
            
            return ServiceResponse<bool>.SuccessResponse(
                statusCode: StatusCodes.Status200OK,
                data: true
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"Error sending email: {e}");
            return ServiceResponse<bool>.FailResponse(
                statusCode: StatusCodes.Status500InternalServerError,
                errorDescription: "Error sending Email."
            );
        }
    }
}
