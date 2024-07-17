namespace ParkSharing.Notification.Server.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailClient _emailClient;
        private readonly IEmailTemplateService _templateService;

        public EmailService(IEmailClient emailClient, IEmailTemplateService templateService)
        {
            _emailClient = emailClient;
            _templateService = templateService;
        }

        public async Task SendTemplatedEmailAsync(string receiver, string subject, string templateName, IDictionary<string, string> values)
        {
            var template = await _templateService.GetTemplateAsync(templateName);
            var body = ReplaceTemplateValues(template, values);
            await _emailClient.SendEmailAsync(receiver, subject, body);
        }

        private string ReplaceTemplateValues(string template, IDictionary<string, string> values)
        {
            foreach (var (key, value) in values)
            {
                template = template.Replace("{" + key + "}", value);
            }
            return template;
        }
    }
}
