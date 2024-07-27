using App.Context.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkSharing.Contracts;

namespace App.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IParkingSpotService _parkingSpotService;
        private readonly ILogger<AdminController> _log;
        public AdminController(IParkingSpotService parkingSpotService, ILogger<AdminController> log)
        {
            _parkingSpotService = parkingSpotService;
            _log = log;
        }

        /// <summary>
        /// This method is used for automated UI tests
        /// </summary>
        /// <returns></returns>
        [HttpDelete("deletesettings")]
        [Authorize("write:admin-deletesettings")]
        public async Task<ActionResult> DeleteSettings()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var spot = await _parkingSpotService.GetSpotByUser(userId);
            if (spot == null)
            {
                return NotFound();
            }

            spot.Name = "";
            spot.BankAccount = "";
            spot.PricePerHour = 0;
            spot.Phone = "";
            spot.Availability = new List<Availability>();
            await _parkingSpotService.UpdateSpot(spot);
            await _parkingSpotService.UpdateAvailabilityByUser(userId, spot.Availability);

            if (spot.Reservations != null)
            {
                foreach (var reservation in spot.Reservations)
                {
                    reservation.State = ReservationState.Rejected;
                    await _parkingSpotService.RemoveReservation(reservation.PublicId);
                }
            }

            return Ok();
        }

        [HttpGet("spots")]
        [Authorize("read:admin-reservations")]
        public async Task<ActionResult<List<ParkingSpotDto>>> GetParkingSpots()
        {
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
                    Start = a.StartDate == null ? DateTime.UtcNow.Date.Add(a.StartTime) : a.StartDate.Value.Add(a.StartTime),
                    End = a.EndDate == null ? DateTime.UtcNow.Date.Add(a.EndTime) : a.EndDate.Value.Add(a.EndTime),
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
