using Ecommerce.Utilities;

namespace Ecommerce.Services.Interfaces;

public interface IEmailService
{
    Task<IServiceResponse<bool>> SendEmail(EmailDto request);
    public void CheckEmailConfig();
}