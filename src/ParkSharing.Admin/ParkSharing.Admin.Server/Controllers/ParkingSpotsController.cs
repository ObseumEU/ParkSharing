using App.Context.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using System;

[Route("api/[controller]")]
[ApiController]
public class ParkingSpotController : ControllerBase
{
    private static List<ParkingSpot> _parkingSpots = new List<ParkingSpot>()
    {
        new ParkingSpot()
        {
            Id = 4,
            UserId = "google-oauth2|106383545592871849353"
        }
    };

    [HttpGet]
    [Authorize]
    public ActionResult<ParkingSpotDto> GetParkingSpot()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = _parkingSpots.FirstOrDefault(p => p.UserId == userId);
        var result = TinyMapper.Map<ParkingSpotDto>(spot);
        return result;
    }

    [Authorize]
    [HttpPut]
    public IActionResult UpdateAvaliability(PutAvaliabilityDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = _parkingSpots.FirstOrDefault(p => p.UserId == userId);
        if (spot == null)
        {
            return NotFound();
        }

        var updatedSpot = dto.Availability;

        //spot.Name = updatedSpot.Name;
        //spot.BankAccount = updatedSpot.BankAccount;
        spot.Availability = new List<Availability>();
        foreach (var av in updatedSpot)
        {
            spot.Availability.Add(TinyMapper.Map<Availability>(av));
        }
        //TODO SAVE
        return NoContent();
    }
}
