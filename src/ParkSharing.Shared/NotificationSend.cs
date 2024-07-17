using System.ComponentModel.DataAnnotations;

namespace ParkSharing.Contracts
{
    public record NotificationSend
    {
        [property: Required]
        public NotificationType Type { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public string TemplateName { get; set; }
    }

    public enum NotificationType
    {
        Email = 0
    }
}
