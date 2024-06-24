using OpenAI.ObjectModels.RequestModels;

namespace ParkSharing.Reservation.Server.Services.Session
{
    public interface ISessionService
    {
        Task AddMessage(string publicId, ChatMessage message);
        Task<Model.Session> CreateSession(string publicId);
        Task DeleteSession(string publicId);
        Task<List<ChatMessage>> GetAllMessages(string publicId, int limit = 10);
        Task<Model.Session> GetSession(string publicId, int limit = 10);
        public Task UpdateAllMessages(string publicId, List<ChatMessage> newMessages);
    }
}