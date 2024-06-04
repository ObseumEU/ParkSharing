using App.Context.Models;
using MassTransit;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MongoDB.Driver;
using System.Numerics;

public class DebugSeedData
{
    IBus _messageBroker;
    IReservationService _reservation;
    public DebugSeedData(IBus messageBroker, IReservationService reservation)
    {
        _messageBroker = messageBroker;
        _reservation = reservation;
    }

    public async Task InitializeAsync()
    {
        Task.Run(async () => {
            await Task.Delay(5000);
            await _reservation.ReserveAsync("GS22", new ReservationSpot()
            {
                PublicId = Guid.NewGuid().ToString(),
                Start = new DateTime(),
                End = new DateTime(),
                Phone = "123123123"
            });
        });
      
    }
}