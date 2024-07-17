namespace ParkSharing.Notification.Server.Email
{
    public interface IEmailClient
    {
        Task SendEmailAsync(string receiver, string subject, string body);
    }
}
