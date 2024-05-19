public class AvailabilityDto
{
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string? Recurrence { get; set; }
    public string? DayOfWeek { get; set; }
}

public class ParkingSpotDto
{
    public string? Name { get; set; }
    public string? BankAccount { get; set; }
    public List<AvailabilityDto>? Availability { get; set; }
}


public class PutAvaliabilityDto
{
    public AvailabilityDto[] Availability { get; set; }
}


