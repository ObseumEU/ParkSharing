using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ParkSharing.Notification.Server.Email
{
    public class EmailClient : IEmailClient
    {
        private readonly IOptionsMonitor<EmailConfig> _options;

        public EmailClient(IOptionsMonitor<EmailConfig> options)
        {
            _options = options;
        }

        public async Task SendEmailAsync(string receiver, string subject, string body)
        {
            using (var client = new SmtpClient(_options.CurrentValue.SmtpServer, _options.CurrentValue.SmtpPort))
            {
                client.Credentials = new NetworkCredential(_options.CurrentValue.SenderEmail, _options.CurrentValue.SenderPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_options.CurrentValue.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(receiver);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
