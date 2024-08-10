using Microsoft.AspNetCore.Mvc;
using ParkSharing;
using ParkSharing.Reservation.Server.Services.Session;
using ParkSharing.Services.ChatGPT;
using System.Security.Cryptography;

[ApiController]
[Route("[controller]")]
public class ParkingController : ControllerBase
{
    private readonly HttpClient _httpClient;
    ChatGPTService _gpt;
    ILogger<ParkingController> _log;
    ISessionService _sessionsService;

    public ParkingController(HttpClient httpClient, ChatGPTService gpt, ILogger<ParkingController> log, ISessionService sessionsService)
    {
        _httpClient = httpClient;
        _gpt = gpt;
        _log = log;
        _sessionsService = sessionsService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserInputModel input)
    {

        var session = await GetOrCreateSession();
        try
        {
            _log.LogInformation($"Session PublicId: {session.PublicId} User Input: {input.Input} ");
            var newMessages = await _gpt.Send(session.Messages, HtmlHelpers.SanitizeHtml(input.Input));
            await _sessionsService.UpdateAllMessages(session.PublicId, newMessages);
            session.Messages.RemoveAll(m => m == null);
            newMessages.RemoveAll(m => m == null);
            return Ok(new { reply = newMessages.LastOrDefault().Content });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Cannot receive message");
            throw ex;
        }
    }

    private async Task<ParkSharing.Reservation.Server.Services.Session.Model.Session> GetOrCreateSession()
    {
        ParkSharing.Reservation.Server.Services.Session.Model.Session session;
        var sessionId = HttpContext.Session.GetString("SessionId");
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = GenerateSecureSessionId();
            HttpContext.Session.SetString("SessionId", sessionId);
            session = await _sessionsService.CreateSession(sessionId);
        }
        else
        {
            session = await _sessionsService.GetSession(sessionId);

            if (session == null)
            {
                session = await _sessionsService.CreateSession(sessionId);
            }
        }
        return session;
    }

    private string GenerateSecureSessionId()
    {
        using (var randomNumberGenerator = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32]; // 256 bits
            randomNumberGenerator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}

public class UserInputModel
{
    public string Input { get; set; }
}
