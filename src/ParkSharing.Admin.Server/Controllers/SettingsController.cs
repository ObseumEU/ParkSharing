using App;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            var spot = await _parkingSpotService.GetOrCreateSpotByUser(userId, email);
            return new SettingsDto()
            {
                BankAccount = Helpers.SanitizeHtml(spot.BankAccount),
                Name = Helpers.SanitizeHtml(spot.Name),
                PricePerHour = spot.PricePerHour,
                Phone = spot.Phone
            };
        }
        catch (Exception ex)
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

        spot.Name = Helpers.SanitizeHtml(dto.Name);
        spot.BankAccount = Helpers.SanitizeHtml(dto.BankAccount);
        spot.PricePerHour = dto.PricePerHour.Value;
        spot.Phone = dto.Phone;
        spot.Email = email;
        spot.UserId = userId;
        await _parkingSpotService.UpdateSpot(spot);
        return NoContent();
    }
}
