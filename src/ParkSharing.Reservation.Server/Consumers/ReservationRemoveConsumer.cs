using App.Context.Models;
using MassTransit;
using MongoDB.Driver;
using Nelibur.ObjectMapper;
using ParkSharing.Contracts;
using System.Diagnostics;

public class ReservationRemoveConsumer : IConsumer<ReservationRemovedEvent>
{
    private readonly IMongoCollection<ParkingSpot> _parkingSpotsCollection;

    public ReservationRemoveConsumer(IMongoDbContext context)
    {
        _parkingSpotsCollection = context.ParkingSpots;

    }

    public async Task Consume(ConsumeContext<ReservationRemovedEvent> context)
    {
        Debug.WriteLine($"Received: {System.Text.Json.JsonSerializer.Serialize(context.Message)}");
        var msg = context.Message;

        // Find the parking spot that contains the reservation with the given PublicId
        var filter = Builders<ParkingSpot>.Filter.ElemMatch(spot => spot.Reservations, reservation => reservation.PublicId == msg.PublicId);
        var update = Builders<ParkingSpot>.Update.PullFilter(spot => spot.Reservations, reservation => reservation.PublicId == msg.PublicId);

        // Apply the update
        var result = await _parkingSpotsCollection.UpdateOneAsync(filter, update);

        // Check if the update was successful
        if (result.ModifiedCount == 0)
        {
            throw new Exception("Reservation not found or removal failed.");
        }
    }
}
