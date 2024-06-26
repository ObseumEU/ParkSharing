﻿using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
