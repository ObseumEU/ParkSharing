using System.ComponentModel.DataAnnotations;

namespace ParkSharing.Contracts
{
    public record ReservationRemovedEvent
    {
        [property: Required]
        public string PublicId { get; set; }
    }
}
