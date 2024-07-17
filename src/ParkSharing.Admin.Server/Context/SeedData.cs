using App.Context.Models;
using App.Services;
using MassTransit;
using ParkSharing.Contracts;

public class DebugSeedData
{
    private readonly IParkingSpotService _parkingSpotService;
    IBus _bus;
    public DebugSeedData(IBus bus, IParkingSpotService parkingSpotService)
    {
        _parkingSpotService = parkingSpotService;
        _bus = bus;
    }

    public async Task InitializeAsync()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(3000);

            var newSpot = await _parkingSpotService.GetOrCreateSpotByUser("google-oauth2|106383545592871849353");

            newSpot.PublicId = "ADebugSpot";
            newSpot.BankAccount = "NL22ABNA0123456789";
            newSpot.Phone = "345342234";
            newSpot.Availability = new List<Availability>
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
                    };
            newSpot.Name = "GS22";
            newSpot.PricePerHour = 30;
            newSpot.UserId = "google-oauth2|106383545592871849353";
            await _parkingSpotService.UpdateSpot(newSpot);
        });
    }
}