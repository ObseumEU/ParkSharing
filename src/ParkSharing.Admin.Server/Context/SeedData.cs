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
        Task.Run(async () =>
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
                            Start = new TimeSpan(8, 0, 0),
                            End = new TimeSpan(18, 0, 0),
                            Recurrence= Recurrence.Daily
                        }
                    },
                Name = "GS22",
                PricePerHour = 5
            };

            await _context.InsertSpot(newParkSpot);
        });
    }
}