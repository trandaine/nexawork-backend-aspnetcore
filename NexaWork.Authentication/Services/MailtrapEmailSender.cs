using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace NexaWork.Authentication.Services
{
    public class MailtrapEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailtrapEmailSender> _logger;

        public MailtrapEmailSender(IConfiguration configuration, ILogger<MailtrapEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailSettings = _configuration.GetSection("MailtrapSettings");
                var host = emailSettings["Host"] ?? "sandbox.smtp.mailtrap.io";
                var port = int.Parse(emailSettings["Port"] ?? "2525");
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("NexaWork Security", "noreply@nexawork.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

                using var client = new SmtpClient();
                // For Mailtrap, we can use StartTls or Auto
                await client.ConnectAsync(host, port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email '{subject}' successfully sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email '{subject}' to {email}");
                throw; // rethrow to handle in UI if necessary
            }
        }
    }
}
