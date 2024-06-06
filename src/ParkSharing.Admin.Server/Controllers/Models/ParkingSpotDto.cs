using ParkSharing.Contracts;

public class AvailabilityDto
{
    public string? Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public AvailabilityRecurrence? Recurrence { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}

public class ParkingSpotDto
{
    public string Id { get; set; }
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string BankAccount { get; set; }
    public List<AvailabilityDto> Availability { get; set; }
    public List<ReservationDto> Reservations { get; set; }
    public decimal PricePerHour { get; set; }
}

public class ReservationDto
{
    public string Id { get; set; }
    public string PublicId { get; set; }
    public string Phone { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public decimal Price { get; set; }
    public ReservationState State { get; set; }
}




//public class AvailabilityDto
//{
//    public DateTime? Start { get; set; }
//    public DateTime? End { get; set; }
//    public string? Recurrence { get; set; }
//    public string? DayOfWeek { get; set; }
//}

//public class ParkingSpotDto
//{
//    public string? Name { get; set; }
//    public string? BankAccount { get; set; }
//    public List<AvailabilityDto>? Availability { get; set; }
//    public List<ReservationDto>? Reservations { get; set; }
//}

//public class ReservationDto
//{
//    public string Id { get; set; }
//    public string Phone { get; set; }
//    public DateTime? Start { get; set; }
//    public DateTime? End { get; set; }
//    public int Price { get; set; }
//    public StateDto State { get; set; }
//}

//public enum StateDto
//{
//    Created,
//    Rejected
//}

public class SettingsDto
{
    public string? Name { get; set; }
    public string? BankAccount { get; set; }
}


public class PutAvaliabilityDto
{
    public AvailabilityDto[] Availability { get; set; }
}


