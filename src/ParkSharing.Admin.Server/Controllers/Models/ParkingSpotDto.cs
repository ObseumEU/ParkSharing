using ParkSharing.Contracts;
using System.ComponentModel.DataAnnotations;

public class AvailabilityDto
{
    [StringLength(100, MinimumLength = 1)]
    public string? PublicId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public AvailabilityRecurrence? Recurrence { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}

public class ParkingSpotDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Id { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string PublicId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }


    [StringLength(60, MinimumLength = 5)]
    public string BankAccount { get; set; }
    public List<AvailabilityDto> Availability { get; set; }
    public List<ReservationDto> Reservations { get; set; }
    public decimal PricePerHour { get; set; }
}

public class ReservationDto
{
    [StringLength(60, MinimumLength = 5)]
    public string Id { get; set; }

    [StringLength(60, MinimumLength = 5)]
    public string PublicId { get; set; }

    [StringLength(60, MinimumLength = 5)]
    public string Phone { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public decimal Price { get; set; }
    public ReservationState State { get; set; }
}



public class SettingsDto
{
    [StringLength(60, MinimumLength = 3)]
    public string? Name { get; set; }

    [StringLength(60, MinimumLength = 3)]
    public string? BankAccount { get; set; }

    public decimal? PricePerHour { get; set; }
    public string? Phone { get; set; }
}


public class PutAvaliabilityDto
{
    public AvailabilityDto[] Availability { get; set; }
}


