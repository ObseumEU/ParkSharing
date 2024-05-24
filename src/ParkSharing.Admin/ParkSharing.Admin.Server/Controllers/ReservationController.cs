using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    IParkingSpotService _parkingSpotService;
    public ReservationController(IParkingSpotService parkingSpotService)
    {
        _parkingSpotService = parkingSpotService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<ReservationDto>?>> GetReservations()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = await _parkingSpotService.GetSpotByUser(userId);
        var result = spot.Reservations?
            .Select(r => TinyMapper.Map<ReservationDto>(r))
            .OrderBy(r => r.Start)
            .ToList();
        return result;
    }
    [Authorize]
    [HttpPut("reject")]
    public async Task<IActionResult> Reject(string reservationId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = await _parkingSpotService.GetSpotByUser(userId);
        if (spot == null)
        {
            return NotFound();
        }

        var reservation = spot.Reservations.FirstOrDefault(r => r.Id == reservationId);

        if (reservation == null)
        {
            return NotFound();
        }

        reservation.State = State.Rejected; // Assuming 1 is the state for rejected
                                            //TODO SAVE in db
        return NoContent();
    }

    [Authorize]
    [HttpPut("allow")]
    public async Task<IActionResult> Allow(string reservationId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = await _parkingSpotService.GetSpotByUser(userId);
        if (spot == null)
        {
            return NotFound();
        }

        var reservation = spot.Reservations.FirstOrDefault(r => r.Id == reservationId);

        if (reservation == null)
        {
            return NotFound();
        }

        reservation.State = State.Created; // Assuming 0 is the state for allowed
                                           //TODO SAVE in db
        return NoContent();
    }
}
