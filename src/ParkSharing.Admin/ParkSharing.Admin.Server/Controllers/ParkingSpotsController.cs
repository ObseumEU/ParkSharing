using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ParkingSpotsController : ControllerBase
{
    // In-memory storage for demonstration purposes
    private static List<ParkingSpotDto> _parkingSpots = new List<ParkingSpotDto>();

    [HttpGet]
    [Authorize]
    public ActionResult<IEnumerable<ParkingSpotDto>> GetParkingSpots()
    {
        return Ok(_parkingSpots);
    }

    [HttpPost]
    [Authorize]
    public ActionResult<ParkingSpotDto> AddParkingSpot(ParkingSpotDto spot)
    {
        spot.Id = _parkingSpots.Count + 1;
        _parkingSpots.Add(spot);
        return CreatedAtAction(nameof(GetParkingSpots), new { id = spot.Id }, spot);
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult UpdateParkingSpot(int id, ParkingSpotDto updatedSpot)
    {
        var spot = _parkingSpots.FirstOrDefault(s => s.Id == id);
        if (spot == null)
        {
            return NotFound();
        }

        spot.Name = updatedSpot.Name;
        spot.BankAccount = updatedSpot.BankAccount;
        spot.Availability = updatedSpot.Availability;
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteParkingSpot(int id)
    {
        var spot = _parkingSpots.FirstOrDefault(s => s.Id == id);
        if (spot == null)
        {
            return NotFound();
        }

        _parkingSpots.Remove(spot);
        return NoContent();
    }
}
