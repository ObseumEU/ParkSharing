using App.Context.Models;
using MassTransit;
using MongoDB.Driver;
using Nelibur.ObjectMapper;
using ParkSharing.Contracts;
using System.Diagnostics;

public class AdminConsumer : IConsumer<ParkSpotCreatedOrUpdatedEvent>
{
    private readonly IMongoCollection<ParkingSpot> _parkingSpotsCollection;

    public AdminConsumer(IMongoDbContext context)
    {
        _parkingSpotsCollection = context.ParkingSpots;

    }

    public async Task Consume(ConsumeContext<ParkSpotCreatedOrUpdatedEvent> context)
    {
        Debug.WriteLine($"Received: {System.Text.Json.JsonSerializer.Serialize(context.Message)}");
        var msg = context.Message;
        var newParkingSpot = new ParkingSpot()
        {
            Availability = msg.Availability.Select(a => new Availability
            {
                DayOfWeek = a.DayOfWeek,
                EndDate = a.EndDate,
                EndTime = a.EndTime,
                PublicId = a.PublicId,
                Recurrence = a.Recurrence,
                StartDate = a.StartDate,
                StartTime = a.StartTime
            }).ToList(),
            BankAccount = msg.BankAccount,
            Name = msg.Name,
            PricePerHour = msg.PricePerHour,
            PublicId = msg.PublicId
        };
        await _parkingSpotsCollection.InsertOneAsync(newParkingSpot);
    }
}
