using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ParkSharing.Contracts;
using System.ComponentModel.DataAnnotations;

public class Availability
{
    [Required]
    public string PublicId { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Required]
    [BsonRepresentation(BsonType.String)]
    public AvailabilityRecurrence? Recurrence { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}

public class ParkingSpot
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [Required]
    public string PublicId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string BankAccount { get; set; }
    public List<Availability> Availability { get; set; }
    public List<ReservationSpot> Reservations { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public decimal PricePerHour { get; set; }
}

public class ReservationSpot
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [Required]
    public string PublicId { get; set; }
    public string Phone { get; set; }
    [Required]
    public DateTime Start { get; set; }
    [Required]
    public DateTime End { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    [BsonRepresentation(BsonType.String)]
    public ReservationState State { get; set; }
}

public class OpenSlot
{
    public DateTime From;
    public DateTime To;
    public string SpotName;
}