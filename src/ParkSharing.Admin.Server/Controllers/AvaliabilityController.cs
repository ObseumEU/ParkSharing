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

        var test = DateTime.Now.AddHours(2);

        ParkingSpotDto res = new ParkingSpotDto()
        {
            BankAccount = spot.BankAccount,
            Name = spot.Name,
            PricePerHour = spot.PricePerHour,
            PublicId = spot.PublicId,
            Id = spot.Id,
        };

        var test2 = DateTime.Now.Date.AddHours(2);


        if (spot.Availability != null)
        {



            res.Availability = new List<AvailabilityDto>();
            foreach (var a in spot.Availability)
            {
                if (a.EndDate != null)
                {
                    a.EndDate = a.EndDate.Value.ToLocalTime();
                }

                if (a.StartDate != null)
                {
                    a.StartDate = a.StartDate.Value.ToLocalTime();
                }

                res.Availability.Add(new AvailabilityDto()
                {
                    PublicId = a.PublicId,
                    DayOfWeek = a.DayOfWeek,
                    End = (a.EndDate == null ? DateTime.Now.ToLocalTime().Date.Add(a.EndTime) : a.EndDate.Value.Date.Add(a.EndTime)).ToLocalTime(),
                    Start = (a.StartDate == null ? DateTime.Now.ToLocalTime().Date.Add(a.StartTime) : a.StartDate.Value.Date.Add(a.StartTime)).ToLocalTime(),
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
            av.Start = av.Start.ToLocalTime();
            av.End = av.End.ToLocalTime();

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
