public class ReservationServiceTests
{
    private ReservationService _reservationService;

    public ReservationServiceTests()
    {
        _reservationService = new ReservationService();
    }

    [Fact]
    public void RegisterOwner_WithValidInput_ShouldSucceed()
    {
        // Arrange
        var phone = "123-456-7890";
        var email = "owner@example.com";
        var parkingSpots = new[] { "Spot 1", "Spot 2" };
        var password = "Password";

        // Act
        var ownerId = _reservationService.RegisterOwner(phone, email, parkingSpots.ToList(), password);

        // Assert
        Assert.NotNull(ownerId);
        var registeredOwner = _reservationService.GetOwnerById(ownerId);
        Assert.Equal(phone, registeredOwner.Phone);
        Assert.Equal(email, registeredOwner.Email);
        Assert.Equal(2, registeredOwner.ParkingSpots.Count);
        Assert.Equal(password, registeredOwner.Password);
    }

    [Fact]
    public void AddAvailability_WithValidInput_ShouldSucceed()
    {
        // Arrange
        var ownerId = _reservationService.RegisterOwner("123-456-7890", "owner@example.com", new[] { "Spot 1" }.ToList(), "password");
        var parkingSpot = _reservationService.GetOwnerById(ownerId).ParkingSpots.First();

        // Act
        var result = _reservationService.AddAvailability(ownerId, parkingSpot.ParkingSpotId, new TimeSpan(10, 0, 0), new TimeSpan(16, 0, 0), DayOfWeek.Monday, null, true);

        // Assert
        Assert.True(result);
        var availability = _reservationService.GetParkingSpotById(ownerId, parkingSpot.ParkingSpotId).Availabilities.First();
        Assert.Equal(new TimeSpan(10, 0, 0), availability.StartTime);
        Assert.Equal(new TimeSpan(16, 0, 0), availability.EndTime);
        Assert.True(availability.IsRecurring);
        Assert.Equal(DayOfWeek.Monday, availability.DayOfWeek);
        Assert.Null(availability.SpecificDate);
    }

    [Fact]
    public void SetPrice_WithValidInput_ShouldUpdatePrice()
    {
        // Arrange
        var ownerId = _reservationService.RegisterOwner("123-456-7890", "owner@example.com", new[] { "Spot 1" }.ToList(), "password");
        var parkingSpot = _reservationService.GetOwnerById(ownerId).ParkingSpots.First();

        // Act
        var result = _reservationService.SetPrice(ownerId, parkingSpot.ParkingSpotId, 20m);

        // Assert
        Assert.True(result);
        var updatedSpot = _reservationService.GetParkingSpotById(ownerId, parkingSpot.ParkingSpotId);
        Assert.Equal(20m, updatedSpot.PricePerHour);
    }

    [Fact]
    public void AddAvailability_WithNonexistentOwner_ShouldFail()
    {
        // Act
        var result = _reservationService.AddAvailability("nonexistent", Guid.NewGuid(), new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), DayOfWeek.Friday, null, false);

        // Assert
        Assert.False(result);
    }
}
