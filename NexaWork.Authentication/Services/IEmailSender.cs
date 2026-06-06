using System.Threading.Tasks;

namespace NexaWork.Authentication.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
