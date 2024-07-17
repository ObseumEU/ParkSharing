namespace ParkSharing.Notification.Server.Email
{
    public interface IEmailTemplateService
    {
        Task<string> GetTemplateAsync(string templateName);
    }
}
