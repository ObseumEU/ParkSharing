using Microsoft.AspNetCore.Mvc;
using OpenAI.ObjectModels.RequestModels;
using ParkSharing.Services.ChatGPT;
using ParkSharing.Services.ChatGPT.Helpers;
using System.Security.Cryptography;

[ApiController]
[Route("[controller]")]
public class ParkingController : ControllerBase
{
    private readonly HttpClient _httpClient;
    ChatGPTService _gpt;
    static List<ChatMessage> messages = new List<ChatMessage>();
    ILogger<ParkingController> _log;

    public ParkingController(HttpClient httpClient, ChatGPTService gpt, ILogger<ParkingController> log)
    {
        _httpClient = httpClient;
        _gpt = gpt;
        _log = log;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserInputModel input)
    {
        var sessionId = GetOrCreateSessionId();
        try
        {
            messages.Add(ChatMessage.FromUser(input.Input));
            messages = await _gpt.Send(messages);
            return Ok(new { reply = messages.LastOrDefault().Content });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Cannot receive messher");
            throw new Exception(); 
        }
    }

    private string GetOrCreateSessionId()
    {
        var sessionId = HttpContext.Session.GetString("SessionId");

        // If not present, generate a new one and set it in the session
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = GenerateSecureSessionId();
            HttpContext.Session.SetString("SessionId", sessionId);
        }

        return sessionId;
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
