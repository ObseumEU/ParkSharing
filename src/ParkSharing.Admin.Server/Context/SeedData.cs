using App.Context.Models;
using MongoDB.Driver;
using System.Numerics;
using System;
using MassTransit;
using App.Services;
using ParkSharing.Contracts;

public class DebugSeedData
{
    private readonly IParkingSpotService _context;
    IBus _bus;
    public DebugSeedData(IBus bus, IParkingSpotService context)
    {
        _context = context;
        _bus = bus;
    }

    public async Task InitializeAsync()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            ParkingSpot newParkSpot = new ParkingSpot()
            {
                PublicId = "ADebugSpot",
                BankAccount = "NL22ABNA0123456789",
                Availability = new List<Availability>
                    {
                        new Availability
                        {
                            Id = Guid.NewGuid().ToString(),
                            StartTime = new TimeSpan(8, 1, 33),
                            EndTime = new TimeSpan(18, 4, 11),
                            Recurrence= AvailabilityRecurrence.Daily
                        },
                        new Availability
                        {
                            Id = Guid.NewGuid().ToString(),
                            StartTime = new TimeSpan(1, 4, 0),
                            EndTime = new TimeSpan(2, 2, 0),
                            Recurrence = AvailabilityRecurrence.Weekly,
                            DayOfWeek = DayOfWeek.Tuesday
                        },
                        new Availability
                        {
                            Id = Guid.NewGuid().ToString(),
                            StartDate = DateTime.UtcNow.AddDays(1).AddHours(2),
                            EndDate = DateTime.UtcNow.AddDays(2).AddHours(4),
                            Recurrence= AvailabilityRecurrence.Once
                        }
                    },
                Name = "GS22",
                PricePerHour = 5,
                UserId = "google-oauth2|106383545592871849353"
            };

            await _context.InsertSpot(newParkSpot);
        });
    }
}