namespace ParkingReservationApp.Services.ChatGPT
{
    public interface IChatGPTClient
    {
        Task<string> CallChatGpt(string text);
    }
}