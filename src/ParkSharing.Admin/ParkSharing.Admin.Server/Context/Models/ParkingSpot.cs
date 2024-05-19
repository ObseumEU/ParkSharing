namespace App.Context.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Recurrence { get; set; }
    }

    public class ParkingSpot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public List<Availability> Availability { get; set; }
        public string UserId { get; set; }
    }
}
