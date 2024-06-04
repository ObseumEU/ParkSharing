using App.Context.Models;
using MassTransit;
using ParkSharing.Contracts;
using System.Xml.Linq;

public class DebugSeedData
{
    IBus _messageBroker;
    public DebugSeedData(IBus messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task InitializeAsync()
    {
        var newReservation = new ReservationCreatedEvent()
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(2),
            PublicSpotId = "ADebugSpot",
            PublicId = Guid.NewGuid().ToString(),
            Phone = "612345678"
        };
    }
}