
namespace ParkSharing.Notification.Server.Email
{
    public interface IEmailService
    {
        Task SendTemplatedEmailAsync(string receiver, string subject, string templateName, IDictionary<string, string> values);
    }
}