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
        var newParkingSpot = TinyMapper.Map<ParkingSpot>(context.Message);
        await _parkingSpotsCollection.InsertOneAsync(newParkingSpot);
    }
}
