using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Context.Models
{
    public class Availability
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public Recurrence? Recurrence { get; set; }
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
        public int Price { get; set; }
        public State State { get; set; }
    }
    public enum State
    {
        Created,
        Rejected
    }

    public enum Recurrence
    {
        Daily,
        Weekly,
        Monthly
    }
}
