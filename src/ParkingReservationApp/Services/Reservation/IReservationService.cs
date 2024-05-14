
public interface IReservationService
{
    bool AddAvailability(string ownerId, Guid parkingSpotId, TimeSpan startTime, TimeSpan endTime, DayOfWeek? dayOfWeek, DateTime? specificDate, bool isRecurring);
    Owner GetOwnerById(string ownerId);
    IEnumerable<Owner> GetOwners();
    ParkingSpot GetParkingSpotById(string ownerId, Guid parkingSpotId);
    string RegisterOwner(string phone, string email, List<string> parkingSpots, string password);
    bool SetPrice(string ownerId, Guid parkingSpotId, decimal pricePerHour);
}