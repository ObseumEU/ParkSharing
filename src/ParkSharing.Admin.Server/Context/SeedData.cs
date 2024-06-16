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
            await Task.Delay(3000);
            ParkingSpot newParkSpot = new ParkingSpot()
            {
                PublicId = "ADebugSpot",
                BankAccount = "NL22ABNA0123456789",
                Availability = new List<Availability>
                    {
                        new Availability
                        {
                            PublicId = Guid.NewGuid().ToString(),
                            StartTime = new TimeSpan(8, 1, 33),
                            EndTime = new TimeSpan(18, 4, 11),
                            Recurrence= AvailabilityRecurrence.Daily
                        },
                        new Availability
                        {
                            PublicId = Guid.NewGuid().ToString(),
                            StartTime = new TimeSpan(1, 4, 0),
                            EndTime = new TimeSpan(2, 2, 0),
                            Recurrence = AvailabilityRecurrence.Weekly,
                            DayOfWeek = DayOfWeek.Tuesday
                        },
                        new Availability
                        {
                            PublicId = Guid.NewGuid().ToString(),
                            StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, DateTime.UtcNow.Day, 10,0,0),
                            EndDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, DateTime.UtcNow.Day, 18,0,0),
                            Recurrence= AvailabilityRecurrence.Once
                        }
                    },
                Name = "GS22",
                PricePerHour = 30,
                UserId = "google-oauth2|106383545592871849353"
            };

            await _context.InsertSpot(newParkSpot);
        });
    }
}