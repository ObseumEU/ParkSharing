﻿using MassTransit;
using MongoDB.Driver;
using ParkSharing.Contracts;
public class AdminConsumer : IConsumer<ParkSpotCreatedOrUpdatedEvent>
{
    private readonly IMongoCollection<ParkingSpot> _parkingSpotsCollection;
    private readonly ILogger<AdminConsumer> _log;

    public AdminConsumer(IMongoDbContext context, ILogger<AdminConsumer> log)
    {
        _parkingSpotsCollection = context.ParkingSpots;
        _log = log;
    }

    public async Task Consume(ConsumeContext<ParkSpotCreatedOrUpdatedEvent> context)
    {
        _log.LogInformation($"Received: {System.Text.Json.JsonSerializer.Serialize(context.Message)}");
        var msg = context.Message;

        var filter = Builders<ParkingSpot>.Filter.Eq(ps => ps.PublicId, msg.PublicId);
        var existingSpot = await _parkingSpotsCollection.Find(filter).FirstOrDefaultAsync();

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

        if (existingSpot != null)
        {
            // Update the existing parking spot
            var update = Builders<ParkingSpot>.Update
                .Set(ps => ps.Availability, newParkingSpot.Availability)
                .Set(ps => ps.BankAccount, newParkingSpot.BankAccount)
                .Set(ps => ps.Name, newParkingSpot.Name)
                .Set(ps => ps.PricePerHour, newParkingSpot.PricePerHour);

            await _parkingSpotsCollection.UpdateOneAsync(filter, update);
        }
        else
        {
            // Insert a new parking spot
            await _parkingSpotsCollection.InsertOneAsync(newParkingSpot);
        }
    }
}
