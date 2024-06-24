using MongoDB.Driver;
using OpenAI.ObjectModels.RequestModels;

namespace ParkSharing.Reservation.Server.Services.Session
{
    public class SessionService : ISessionService
    {
        private readonly IMongoCollection<Model.Session> _sessionsCollection;

        public SessionService(IMongoDbContext dbContext)
        {
            _sessionsCollection = dbContext.Sessions;
        }

        public async Task<List<ChatMessage>> GetAllMessages(string publicId, int limit = 10)
        {
            var filter = Builders<Model.Session>.Filter.Eq(s => s.PublicId, publicId);
            var projection = Builders<Model.Session>.Projection.Slice(s => s.Messages, -limit);
            var session = await _sessionsCollection.Find(filter)
                                                   .Project<Model.Session>(projection)
                                                   .FirstOrDefaultAsync();
            return session?.Messages ?? new List<ChatMessage>();
        }

        public async Task AddMessage(string publicId, ChatMessage message)
        {
            var filter = Builders<Model.Session>.Filter.Eq(s => s.PublicId, publicId);
            var update = Builders<Model.Session>.Update.Push(s => s.Messages, message);
            var options = new FindOneAndUpdateOptions<Model.Session>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };
            await _sessionsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task UpdateAllMessages(string publicId, List<ChatMessage> newMessages)
        {
            var filter = Builders<Model.Session>.Filter.Eq(s => s.PublicId, publicId);
            var update = Builders<Model.Session>.Update.Set(s => s.Messages, newMessages);
            await _sessionsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Model.Session> CreateSession(string publicId)
        {
            var session = new Model.Session { PublicId = publicId, Messages = new List<ChatMessage>() };
            await _sessionsCollection.InsertOneAsync(session);
            return session;
        }

        public async Task<Model.Session> GetSession(string publicId, int limit = 10)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var filter = Builders<Model.Session>.Filter.Eq(s => s.PublicId, publicId);
            var projection = Builders<Model.Session>.Projection.Slice(s => s.Messages, -limit);
            var session = await _sessionsCollection.Find(filter)
                                                   .Project<Model.Session>(projection)
                                                   .FirstOrDefaultAsync();
            return session;
        }

        public async Task DeleteSession(string publicId)
        {
            var filter = Builders<Model.Session>.Filter.Eq(s => s.PublicId, publicId);
            await _sessionsCollection.DeleteOneAsync(filter);
        }
    }
}
