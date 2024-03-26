using Ecommerce.Configuration;
using Ecommerce.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Ecommerce.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfiguration;

    public EmailService(EmailConfiguration emailConfiguration)
    {
        _emailConfiguration = emailConfiguration;
    }

    public void checkEmailConfig()
    {
        Console.WriteLine(_emailConfiguration.SmtpServer);
        return;
    }

    public void SendEmail(EmailDto request)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_emailConfiguration.From));
        email.To.Add(MailboxAddress.Parse(request.Recipient));
        email.Subject = request.Subject;

        email.Body = new TextPart(TextFormat.Html) { Text = request.Message };

        using (var smtp = new SmtpClient())
        {
            smtp.Connect(
                _emailConfiguration.SmtpServer,
                _emailConfiguration.Port,
                SecureSocketOptions.StartTls
            );
            smtp.Authenticate(_emailConfiguration.From, _emailConfiguration.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
