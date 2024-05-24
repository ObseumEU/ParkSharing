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
    public List<ReservationDto>? Reservations { get; set; }
}

public class ReservationDto
{
    public string Id { get; set; }
    public string Phone { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public int Price { get; set; }
    public StateDto State { get; set; }
}

public enum StateDto
{
    Created,
    Rejected
}

public class SettingsDto
{
    public string? Name { get; set; }
    public string? BankAccount { get; set; }
}


public class PutAvaliabilityDto
{
    public AvailabilityDto[] Availability { get; set; }
}


