public interface IReservationService
{
    Task<bool> CheckAvaliabilitySpot(DateTime fromUtc, DateTime toUtc, string spotName);
    Task<ParkingSpot> GetParkingSpotAsync(Guid parkingSpotId);
    Task<ParkingSpot> GetParkingSpotByNameAsync(string name);
    Task<bool> RemoveReservationAsync(Guid reservationId);
    Task<bool> ReserveAsync(string spotName, ReservationSpot reservation, bool force = false);
    Task<List<FreeSlot>> GetAllOpenSlots(DateTime fromUtc, DateTime toUtc);
    Task<List<ParkingSpot>> GetAllSpots();
}