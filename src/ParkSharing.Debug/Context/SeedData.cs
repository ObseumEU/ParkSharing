using App.Context.Models;
using MassTransit;
using ParkSharing.Contracts;

public class DebugSeedData
{
    IBus _messageBroker;
    public DebugSeedData(IBus messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task InitializeAsync()
    {
        ParkSpotCreatedOrUpdatedEvent newParkSpot = new ParkSpotCreatedOrUpdatedEvent();
        var spotId = Guid.NewGuid().ToString();
        await _messageBroker.Publish(new ParkSpotCreatedOrUpdatedEvent()
        {
            PublicId = spotId,
            BankAccount = "NL22ABNA0123456789",
            Availability = new List<AvailabilityCreatedOrUpdatedEvent>
                    {
                        new AvailabilityCreatedOrUpdatedEvent
                        {
                            Start = new TimeSpan(8, 0, 0),
                            End = new TimeSpan(18, 0, 0),
                            Recurrence= RecurrenceCreatedOrUpdatedEvent.Daily
                        }
                    },
            Name = "GS22",
            PricePerHour = 5

        });

        await _messageBroker.Publish(new ReservationCreatedEvent()
        {
            PublicId = Guid.NewGuid().ToString(),
            Start = new DateTime(2024, 12, 31, 10, 0, 0),
            End = new DateTime(2024, 12, 31, 12, 0, 0),
            Phone = "123123123",
            PublicSpotId = spotId
        });
    }
}