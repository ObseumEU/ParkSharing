namespace App.Context.Models
{
    public class ReservationCreatedEvent
    {
        public string PublicSpotId { get; set; }
        public string PublicId { get; set; }
        public string ClientPhone { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public decimal Price { get; set; }
    }
}
