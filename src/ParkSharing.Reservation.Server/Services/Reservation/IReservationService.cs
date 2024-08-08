public interface IReservationService
{
    Task<bool> CheckAvaliabilitySpot(DateTime from, DateTime to, string spotName);
    Task<ParkingSpot> GetParkingSpotAsync(Guid parkingSpotId);
    Task<ParkingSpot> GetParkingSpotByNameAsync(string name);
    Task<bool> RemoveReservationAsync(Guid reservationId);
    Task<bool> ReserveAsync(string spotName, ReservationSpot reservation, bool force = false);
    Task<List<FreeSlot>> GetAllOpenSlots(DateTime from, DateTime to);
    Task<List<ParkingSpot>> GetAllSpots();
}