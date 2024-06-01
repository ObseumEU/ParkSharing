using App.Context.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using System;

[Route("api/[controller]")]
[ApiController]
public class AvaliabilityController : ControllerBase
{
    IParkingSpotService _parkingSpotService;
    public AvaliabilityController(IParkingSpotService parkingSpotService)
    {
        _parkingSpotService = parkingSpotService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ParkingSpotDto>> GetParkingSpot()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = await _parkingSpotService.GetSpotByUser(userId);
        var result = TinyMapper.Map<ParkingSpotDto>(spot);
        return result;
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateAvaliability(PutAvaliabilityDto dto)
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
