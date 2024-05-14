public class ReservationService : IReservationService
{
    private List<Owner> Owners = new List<Owner>();

    public string RegisterOwner(string phone, string email, List<string> parkingSpots, string password)
    {
        var newOwner = new Owner
        {
            OwnerId = Guid.NewGuid().ToString(),
            Phone = phone,
            Email = email,
            ParkingSpots = parkingSpots.Select(name => new ParkingSpot { Name = name, ParkingSpotId = Guid.NewGuid() }).ToList(),
            Password = password
        };

        Owners.Add(newOwner);
        return newOwner.OwnerId;
    }

    public bool AddAvailability(string ownerId, Guid parkingSpotId, TimeSpan startTime, TimeSpan endTime, DayOfWeek? dayOfWeek, DateTime? specificDate, bool isRecurring)
    {
        var owner = Owners.FirstOrDefault(o => o.OwnerId == ownerId);
        if (owner == null) return false;

        var parkingSpot = owner.ParkingSpots.FirstOrDefault(ps => ps.ParkingSpotId == parkingSpotId);
        if (parkingSpot == null) return false;

        parkingSpot.Availabilities.Add(new Availability
        {
            AvailabilityId = Guid.NewGuid(),
            DayOfWeek = dayOfWeek,
            SpecificDate = specificDate,
            StartTime = startTime,
            EndTime = endTime,
            IsRecurring = isRecurring
        });

        return true;
    }

    public bool SetPrice(string ownerId, Guid parkingSpotId, decimal pricePerHour)
    {
        var owner = Owners.FirstOrDefault(o => o.OwnerId == ownerId);
        if (owner == null) return false;

        var parkingSpot = owner.ParkingSpots.FirstOrDefault(ps => ps.ParkingSpotId == parkingSpotId);
        if (parkingSpot == null) return false;

        parkingSpot.PricePerHour = pricePerHour;
        return true;
    }

    public IEnumerable<Owner> GetOwners() => Owners;

    public Owner GetOwnerById(string ownerId) => Owners.FirstOrDefault(o => o.OwnerId == ownerId);

    public ParkingSpot GetParkingSpotById(string ownerId, Guid parkingSpotId)
    {
        return Owners.FirstOrDefault(o => o.OwnerId == ownerId)?.ParkingSpots.FirstOrDefault(ps => ps.ParkingSpotId == parkingSpotId);
    }
}
