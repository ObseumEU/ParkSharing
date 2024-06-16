using App.Context.Models;
using MassTransit;

public class DebugSeedData
{
    IBus _messageBroker;
    IReservationService _reservation;
    public DebugSeedData(IBusControl messageBroker, IReservationService reservation)
    {
        _messageBroker = messageBroker;
        _reservation = reservation;
    }

    public async Task InitializeAsync()
    {
        Task.Run(async () => {
            bool res = false;

            for(int i = 0; i < 4 && res == false; i++)
            {
                await Task.Delay(3000);
                res = await _reservation.ReserveAsync("GS22", new ReservationSpot()
                {
                    PublicId = Guid.NewGuid().ToString(),
                    Start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, DateTime.UtcNow.Day, 11, 0, 0),
                    End = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, DateTime.UtcNow.Day, 17, 0, 0),
                    Phone = "123123123",
                    Price = 22,
                    State = ParkSharing.Contracts.ReservationState.Created
                },
                true);
            }
        });
    }
}