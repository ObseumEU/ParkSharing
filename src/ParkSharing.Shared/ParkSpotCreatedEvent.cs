using System.ComponentModel.DataAnnotations;

namespace ParkSharing.Contracts
{
    public record ParkSpotCreatedOrUpdatedEvent
    {
        [property: Required]
        public string PublicId { get; set; }
        [property: Required]
        public string Name { get; set; }
        [property: Required]
        public string BankAccount { get; set; }
        public List<AvailabilityCreatedOrUpdatedEvent> Availability { get; set; }
        [property: Required]
        public decimal PricePerHour { get; set; }
        [property: Required]
        public string Phone { get; set; }
    }

    public record AvailabilityCreatedOrUpdatedEvent
    {
        [property: Required]
        public string PublicId { get; set; }
        [property: Required]
        public TimeSpan StartTime { get; set; }
        [property: Required]
        public TimeSpan EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [property: Required]
        public AvailabilityRecurrence? Recurrence { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
    }

}
