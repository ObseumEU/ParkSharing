namespace ParkSharing.Services.ChatGPT
{
    public interface IChatGPTClient
    {
        Task<string> CallChatGpt(string text);
    }
}