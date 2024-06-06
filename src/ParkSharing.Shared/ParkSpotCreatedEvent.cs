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
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public AvailabilityRecurrence? Recurrence { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
    }

}
