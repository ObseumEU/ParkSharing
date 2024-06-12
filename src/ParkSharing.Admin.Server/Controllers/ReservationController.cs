using App.Context.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using ParkSharing.Contracts;

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
    public async Task<IActionResult> Reject([FromQuery] string reservationId)
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

        var reservation = spot.Reservations.FirstOrDefault(r => r.PublicId == reservationId);

        if (reservation == null)
        {
            return NotFound();
        }

        reservation.State = ReservationState.Rejected;
        await _parkingSpotService.RemoveReservation(reservation.PublicId);
        return NoContent();
    }
}
