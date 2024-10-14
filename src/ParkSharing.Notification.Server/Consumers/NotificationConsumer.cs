using App.Context.Models;
using MassTransit;
using Microsoft.FeatureManagement;
using ParkSharing.Notification.Server.Email;
using ParkSharing.Notification.Server.SMS;
using ParkSharing.Notification.Server.Services;

namespace ParkSharing.Notification.Server.Consumers
{
    public class NotificationConsumer : IConsumer<ReservationCreatedEvent>
    {
        private readonly ILogger<NotificationConsumer> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserInfoService _userService;
        private readonly IFeatureManager _feature;
        private bool _enableNotifications;
        private readonly SMSClient _smsClient;

        public NotificationConsumer(
            ILogger<NotificationConsumer> logger,
            IEmailService emailService,
            IUserInfoService userService,
            SMSClient smsClient,
            IFeatureManager feature)
        {
            _logger = logger;
            _emailService = emailService;
            _userService = userService;
            _smsClient = smsClient;
            _feature = feature;
        }

        public async Task Consume(ConsumeContext<ReservationCreatedEvent> context)
        {
            await SendNotificationAsync(context);
        }

        private async Task SendNotificationAsync(ConsumeContext<ReservationCreatedEvent> context)
        {
            var userInfo = await _userService.GetUserInfo(context.Message.PublicSpotId);
            var values = new Dictionary<string, string>
            {
                ["start"] = context.Message.Start.Value.ToString("d MMMM HH:mm"),
                ["end"] = context.Message.End.Value.ToString("d MMMM HH:mm"),
                ["phone"] = context.Message.ClientPhone,
                ["price"] = context.Message.Price.ToString()
            };

            try
            {
                // Send Email
                if (await _feature.IsEnabledAsync(FeatureFlags.EmailNotifications))
                {
                    await _emailService.SendTemplatedEmailAsync(
                        userInfo.Email,
                        "Místo bylo zarezervováno",
                        "Reservation",
                        values);
                    _logger.LogInformation($"Email sent to {userInfo.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Email notification");
            }

            try
            {
                // Send SMS
                if (await _feature.IsEnabledAsync(FeatureFlags.SMSNotifications))
                {
                    var smsBody =
                        $"Vaše rezervace místa {userInfo.SpotName} od {values["start"]} do {values["end"]} je potvrzena. Cena: {values["price"]} Kč. Kontakt na majitele: {userInfo.Phone}. Zaplaťte prosím na {userInfo.BankAccount}.";
                    await _smsClient.SendSmsAsync(context.Message.ClientPhone, smsBody);
                    _logger.LogInformation($"SMS sent to {context.Message.ClientPhone}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS notification");
            }
        }
    }
}
