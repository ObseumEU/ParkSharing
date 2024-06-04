namespace ParkSharing.Contracts
{
    public class ParkSpotCreatedOrUpdatedEvent
    {
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public List<AvailabilityCreatedOrUpdatedEvent> Availability { get; set; }
        public decimal PricePerHour { get; set; }
    }

    public class AvailabilityCreatedOrUpdatedEvent
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public RecurrenceCreatedOrUpdatedEvent? Recurrence { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
    }

    public enum RecurrenceCreatedOrUpdatedEvent
    {
        Daily,
        Weekly,
        Monthly
    }
}
