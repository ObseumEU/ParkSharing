public class Owner
{
    public string OwnerId { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public List<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();
    public string Password { get; set; }
}
