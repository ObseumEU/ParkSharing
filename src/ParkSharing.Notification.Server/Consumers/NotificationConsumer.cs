using App.Context.Models;
using MassTransit;
using ParkSharing.Notification.Server.Email;
using ParkSharing.Notification.Server.Services;

public class NotificationConsumer : IConsumer<ReservationCreatedEvent>
{
    ILogger<NotificationConsumer> _logger;
    IEmailService _emailService;
    IUserInfoService _userService;
    public NotificationConsumer(ILogger<NotificationConsumer> logger, IEmailService emailService, IUserInfoService userService)
    {
        _logger = logger;
        _userService = userService;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<ReservationCreatedEvent> context)
    {
        //There is for now, only one template called "ReservationCreated". Its harcoded in string variable.
        var userInfo = await _userService.GetUserInfo(context.Message.PublicSpotId);
        var values = new Dictionary<string, string>();
        values["start"] = context.Message.Start.Value.ToLocalTime().ToString("d MMMM HH:mm");
        values["end"] = context.Message.End.Value.ToLocalTime().ToString("d MMMM HH:mm");
        values["phone"] = context.Message.Phone;
        values["price"] = context.Message.Price.ToString();
        await _emailService.SendTemplatedEmailAsync(userInfo.Email, "Místo bylo zarezervováno", "Reservation",values);
    }
}