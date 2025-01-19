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
        Task.Run(async () =>
        {
            bool res = false;

            for (int i = 0; i < 4 && res == false; i++)
            {
                await Task.Delay(3000);
                res = await _reservation.ReserveAsync("GS22", new ReservationSpot()
                {
                    PublicId = Guid.NewGuid().ToString(),
                    Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0).AddMonths(1),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0).AddMonths(1),
                    ClientPhone = "+420724676829",
                    Price = 22,
                    State = ParkSharing.Contracts.ReservationState.Created
                },
                true);
            }
        });
    }
}