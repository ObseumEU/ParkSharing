using MassTransit;
using ParkSharing.Contracts;

public class NotificationConsumer : IConsumer<NotificationSend>
{
    ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(ILogger<NotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationSend> context)
    {
        //There is for now, only one template called "ReservationCreated". Its harcoded in string variable.

        throw new NotImplementedException();
    }
}