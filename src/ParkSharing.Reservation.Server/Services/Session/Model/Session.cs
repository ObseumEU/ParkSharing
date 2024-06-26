using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OpenAI.ObjectModels.RequestModels;

namespace ParkSharing.Reservation.Server.Services.Session.Model
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string Id { get; set; }

        [BsonElement("PublicId")]
        public string PublicId { get; set; }

        [BsonElement("Messages")]
        public List<ChatMessage> Messages { get; set; }
    }
}
