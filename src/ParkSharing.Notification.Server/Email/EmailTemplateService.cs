namespace ParkSharing.Notification.Server.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly Dictionary<string, string> _templates = new()
        {
            {"Reservation", "<p>Proběhla rezervace vašeho parkovacího místa. Od {start} do {end}, cena {price}. <br/> Kontakt na parkujícího {phone}</p>"},
        };

        public Task<string> GetTemplateAsync(string templateName)
        {
            _templates.TryGetValue(templateName, out var template);
            return Task.FromResult(template ?? string.Empty);
        }
    }
}
