namespace ParkSharing.Contracts
{
    public enum ReservationState
    {
        Created,
        Rejected
    }
    public enum AvailabilityRecurrence
    {
        Once = 0,
        Daily = 1,
        Weekly = 2,
        WeekDays = 3
    }
}
