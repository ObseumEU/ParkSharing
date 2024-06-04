using App.Context.Models;

public interface IReservationService
{
    Task<List<ParkingSpot>> GetAvailableSpotsAsync(DateTime fromUtc, DateTime toUtc);
    Task<ParkingSpot> GetParkingSpotAsync(Guid parkingSpotId);
    Task<ParkingSpot> GetParkingSpotByNameAsync(string name);
    Task<bool> RemoveReservationAsync(Guid reservationId);
    Task<bool> ReserveAsync(string spotName, ReservationSpot reservation);
}