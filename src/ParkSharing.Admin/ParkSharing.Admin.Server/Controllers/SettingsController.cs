using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using System;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    IParkingSpotService _parkingSpotService;
    public SettingsController(IParkingSpotService parkingSpotService)
    {
        _parkingSpotService = parkingSpotService;
    }


    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SettingsDto>> GetSettings()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var spot = await _parkingSpotService.GetSpotByUser(userId);
        return new SettingsDto()
        {
            BankAccount = spot.BankAccount,
            Name = spot.Name
        };
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateSettings(SettingsDto dto)
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

        spot.Name = dto.Name;
        spot.BankAccount = dto.BankAccount;
        return NoContent();
    }
}
