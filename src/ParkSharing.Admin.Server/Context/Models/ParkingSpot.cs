using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ParkSharing.Contracts;

namespace App.Context.Models
{
    public class Availability
    {
        public string PublicId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [BsonRepresentation(BsonType.String)]
        public AvailabilityRecurrence? Recurrence { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
    }

    public class ParkingSpot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public List<Availability> Availability { get; set; }
        public List<Reservation> Reservations { get; set; }
        public string UserId { get; set; }
        public decimal PricePerHour { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PublicId { get; set; }
        public string Phone { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public decimal Price { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ReservationState State { get; set; }
    }
}
