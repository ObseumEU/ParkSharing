public class ParkingSpot
{
    public Guid ParkingSpotId { get; set; }
    public string Name { get; set; }
    public decimal PricePerHour { get; set; }
    public List<Availability> Availabilities { get; set; } = new List<Availability>();
}

public class Availability
{
    public Guid AvailabilityId { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateTime? SpecificDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsRecurring { get; set; }
}