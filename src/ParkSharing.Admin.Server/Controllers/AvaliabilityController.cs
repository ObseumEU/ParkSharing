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

        if(spot == null)
        {
            return NotFound();
        }

        ParkingSpotDto res = new ParkingSpotDto()
        {
            BankAccount = spot.BankAccount,
            Name = spot.Name,
            PricePerHour = spot.PricePerHour,
            PublicId = spot.PublicId,
            Id = spot.Id,
        };

        if(spot.Availability != null)
        {
            res.Availability = new List<AvailabilityDto>();
            foreach (var a in spot.Availability)
            {
                res.Availability.Add(new AvailabilityDto()
                {
                    Id = a.Id,
                    DayOfWeek = a.DayOfWeek,
                    End = a.EndDate == null ? DateTime.UtcNow.Add(a.EndTime) : a.EndDate.Value.Add(a.EndTime),
                    Start = a.StartDate == null ? DateTime.UtcNow.Add(a.StartTime) : a.StartDate.Value.Add(a.StartTime),
                    Recurrence = a.Recurrence
                });
            }
        }
        return res;
        //return new ParkingSpotDto()
        //{
        //    Availability = spot.Availability.Select(a => new AvailabilityDto()
        //    {
        //        DayOfWeek = a.DayOfWeek
        //    }),
        //    BankAccount = spot.BankAccount,
        //    Name = spot.Name,
        //    Reservations = spot.Reservations.Select()
        //};
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

        await _parkingSpotService.UpdateAvailabilityByUser(userId, spot.Availability);
        return NoContent();
    }
}
