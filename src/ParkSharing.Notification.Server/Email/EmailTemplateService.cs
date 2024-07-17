namespace ParkSharing.Notification.Server.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly Dictionary<string, string> _templates = new()
        {
            {"Welcome", "<h1>Welcome, {name}!</h1><p>Thank you for joining us.</p>"},
        };

        public Task<string> GetTemplateAsync(string templateName)
        {
            _templates.TryGetValue(templateName, out var template);
            return Task.FromResult(template ?? string.Empty);
        }
    }
}
