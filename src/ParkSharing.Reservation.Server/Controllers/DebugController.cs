using Microsoft.AspNetCore.Mvc;
using OpenAI.ObjectModels.RequestModels;
using ParkSharing.Services.ChatGPT;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly HttpClient _httpClient;
    ChatGPTService _gpt;
    static List<ChatMessage> messages = new List<ChatMessage>();
    ILogger<ParkingController> _log;
    IReservationService _reservations;

    public DebugController(HttpClient httpClient, ChatGPTService gpt, ILogger<ParkingController> log, IReservationService reservations)
    {
        _httpClient = httpClient;
        _gpt = gpt;
        _log = log;
        _reservations = reservations;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParkingSpot>>> Get()
    {
        _log.LogWarning("Called Debug Controller");
        return Ok(await _reservations.GetAllSpots());
    }
}