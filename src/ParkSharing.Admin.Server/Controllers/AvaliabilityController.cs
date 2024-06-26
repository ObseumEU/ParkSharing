using App.Context.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
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

        var spot = await _parkingSpotService.GetOrCreateSpotByUser(userId);

        if (spot == null)
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

        if (spot.Availability != null)
        {
            res.Availability = new List<AvailabilityDto>();
            foreach (var a in spot.Availability)
            {
                res.Availability.Add(new AvailabilityDto()
                {
                    PublicId = a.PublicId,
                    DayOfWeek = a.DayOfWeek,
                    End = a.EndDate == null ? DateTime.UtcNow.Date.Add(a.EndTime) : a.EndDate.Value.Date.Add(a.EndTime),
                    Start = a.StartDate == null ? DateTime.UtcNow.Date.Add(a.StartTime) : a.StartDate.Value.Date.Add(a.StartTime),
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
            spot.Availability.Add(new Availability()
            {
                DayOfWeek = av.DayOfWeek,
                EndDate = av.End.Date,
                StartDate = av.Start.Date,
                EndTime = av.End.TimeOfDay,
                StartTime = av.Start.TimeOfDay,
                PublicId = av.PublicId,
                Recurrence = av.Recurrence
            });
        }

        await _parkingSpotService.UpdateAvailabilityByUser(userId, spot.Availability);
        return NoContent();
    }
}
