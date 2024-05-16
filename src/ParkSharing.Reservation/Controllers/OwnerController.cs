// File: src/ParkSharing/Controllers/OwnerController.cs

using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("[controller]")]
public class OwnerController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public OwnerController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("spots")]
    public IActionResult GetParkingSpots()
    {
        //var ownerId = HttpContext.Session.GetString("SessionId");
        //if (string.IsNullOrEmpty(ownerId))
        //{
        //    return Unauthorized();
        //}

        //var owner = _reservationService.GetOwnerById(ownerId);
        //if (owner == null)
        //{
        //    return NotFound();
        //}
        var owner = new Owner()
        {
            Email = "sadsa",
            OwnerId = "OwnerId",
            Phone = "723556222",
            Password = "Passwd",
            ParkingSpots = new List<ParkingSpot>()
            {
                new ParkingSpot()
                {
                    Name = "asds",
                    Availabilities = new List<Availability>()
                    {
                        new Availability()
                        {
                            AvailabilityId = Guid.NewGuid(),
                            DayOfWeek = DayOfWeek.Monday,
                            IsRecurring = true,
                            StartTime = new TimeSpan(10, 0, 0),
                            EndTime = new TimeSpan(12,0,0)
                        }
                    }
                }
            }
        };
        return Ok(owner.ParkingSpots);
    }

    [HttpPost("spots/{spotId}/availability")]
    public IActionResult AddAvailability(Guid spotId, [FromBody] AvailabilityModel model)
    {
        var ownerId = HttpContext.Session.GetString("SessionId");
        if (string.IsNullOrEmpty(ownerId))
        {
            return Unauthorized();
        }

        var success = _reservationService.AddAvailability(ownerId, spotId, model.StartTime, model.EndTime, model.DayOfWeek, model.SpecificDate, model.IsRecurring);
        if (!success)
        {
            return BadRequest();
        }

        var owner = _reservationService.GetOwnerById(ownerId);
        return Ok(owner.ParkingSpots.FirstOrDefault(ps => ps.ParkingSpotId == spotId));
    }
}

public class AvailabilityModel
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateTime? SpecificDate { get; set; }
    public bool IsRecurring { get; set; }
}
