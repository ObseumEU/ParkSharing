using App;
using App.Context.Models;
using App.Services;
using Auth0.ManagementApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using System;
using System.Text;

[Route("[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    IParkingSpotService _parkingSpotService;
    ILogger<SettingsController> _log;
    public SettingsController(IParkingSpotService parkingSpotService, ILogger<SettingsController> log)
    {
        _parkingSpotService = parkingSpotService;
        _log = log;
    }


    [HttpGet]
    [Authorize]
    public async Task<ActionResult<SettingsDto>> GetSettings()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var spot = await _parkingSpotService.GetOrCreateSpotByUser(userId);
            return new SettingsDto()
            {
                BankAccount = Helpers.SanitizeHtml(spot.BankAccount),
                Name = Helpers.SanitizeHtml(spot.Name),
                PricePerHour = spot.PricePerHour
            };
        }
        catch(Exception ex)
        {
            _log.LogError(ex, "Failed Get settings");
            throw ex;
        }
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

        spot.Name = Helpers.SanitizeHtml(dto.Name);
        spot.BankAccount = Helpers.SanitizeHtml(dto.BankAccount);
        spot.PricePerHour = dto.PricePerHour.Value;
        await _parkingSpotService.UpdateSpot(spot);
        return NoContent();
    }
}
