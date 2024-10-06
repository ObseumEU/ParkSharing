using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;

namespace ParkSharing.Notification.Server.SMS
{
    public class SMSClient
    {
        private readonly ILogger<SMSClient> _logger;
        private readonly IOptionsMonitor<TwilioOptions> _twilioOptions;
        private readonly HttpClient _httpClient;

        public SMSClient(
            ILogger<SMSClient> logger,
            IOptionsMonitor<TwilioOptions> twilioOptions,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _twilioOptions = twilioOptions;
            _httpClient = httpClientFactory.CreateClient("TwilioClient");
        }

        /// <summary>
        /// Sends an SMS message using Twilio's API.
        /// </summary>
        /// <param name="to">Recipient's
        /// number (e.g., "+18777804236").</param>
        /// <param name="body">The message content.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the SMS sending fails.</exception>
        public async Task SendSmsAsync(string to, string body)
        {
            var accountSid = _twilioOptions.CurrentValue.AccountSid;
            var authToken = _twilioOptions.CurrentValue.AuthToken;
            var from = _twilioOptions.CurrentValue.FromNumber;

            if (string.IsNullOrWhiteSpace(accountSid) ||
                string.IsNullOrWhiteSpace(authToken) ||
                string.IsNullOrWhiteSpace(from))
            {
                _logger.LogError("Twilio configuration is missing or incomplete.");
                throw new InvalidOperationException("Twilio configuration is missing or incomplete.");
            }

            var url = $"Accounts/{accountSid}/Messages.json";

            var parameters = new Dictionary<string, string>
            {
                { "To", to },
                { "From", from },
                { "Body", body }
            };

            var content = new FormUrlEncodedContent(parameters);

            // Prepare Basic Authentication header
            var authString = $"{accountSid}:{authToken}";
            var authBytes = Encoding.ASCII.GetBytes(authString);
            var authHeader = Convert.ToBase64String(authBytes);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SMS successfully sent to {To}.", to);
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send SMS. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
                    throw new Exception($"Failed to send SMS: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while sending SMS to {To}.", to);
                throw;
            }
        }
    }
}
