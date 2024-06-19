using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IParkingSpotService _parkingSpotService;

        public AdminController(IParkingSpotService parkingSpotService)
        {
            _parkingSpotService = parkingSpotService;
        }

        [HttpGet("spots")]
        [Authorize("read:admin-reservations")]
        public async Task<ActionResult<List<ParkingSpotDto>>> GetParkingSpots()
        {

            var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var spots = await _parkingSpotService.GetAllSpots();
            var spotDtos = spots.Select(spot => new ParkingSpotDto
            {
                Id = spot.Id,
                PublicId = spot.PublicId,
                Name = spot.Name,
                BankAccount = spot.BankAccount,
                PricePerHour = spot.PricePerHour,
                Availability = spot.Availability?.Select(a => new AvailabilityDto
                {
                    PublicId = a.PublicId,
                    Start = a.StartDate ?? DateTime.UtcNow.Date.Add(a.StartTime),
                    End = a.EndDate ?? DateTime.UtcNow.Date.Add(a.EndTime),
                    Recurrence = a.Recurrence,
                    DayOfWeek = a.DayOfWeek
                }).ToList(),
                Reservations = spot.Reservations?.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    PublicId = r.PublicId,
                    Phone = r.Phone,
                    Start = r.Start,
                    End = r.End,
                    Price = r.Price,
                    State = r.State
                }).ToList()
            }).ToList();

            return Ok(spotDtos);
        }
    }
}
