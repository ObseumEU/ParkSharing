using App.Context.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services
{
    public interface IParkingSpotService
    {
        Task<ParkingSpot> GetSpotByUser(string userId);
        Task<List<Availability>> GetAvailabilityByUser(string userId);
        Task UpdateSpot(ParkingSpot spot);
        Task<List<Availability>> UpdateAvailabilityByUser(string userId, List<Availability> availability);
    }

    public class ParkingSpotServiceMongo : IParkingSpotService
    {
        private readonly IMongoCollection<ParkingSpot> _parkingSpots;

        public ParkingSpotServiceMongo(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ParkingDB");
            _parkingSpots = database.GetCollection<ParkingSpot>("ParkingSpots");
        }

        public async Task<ParkingSpot> GetSpotByUser(string userId)
        {
            var filter = Builders<ParkingSpot>.Filter.Eq(s => s.UserId, userId);
            return await _parkingSpots.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Availability>> GetAvailabilityByUser(string userId)
        {
            var spot = await GetSpotByUser(userId);
            return spot?.Availability ?? new List<Availability>();
        }

        public async Task UpdateSpot(ParkingSpot spot)
        {
            var filter = Builders<ParkingSpot>.Filter.Eq(s => s.Id, spot.Id);
            var options = new ReplaceOptions { IsUpsert = true };
            await _parkingSpots.ReplaceOneAsync(filter, spot, options);
        }

        public async Task<List<Availability>> UpdateAvailabilityByUser(string userId, List<Availability> availability)
        {
            var filter = Builders<ParkingSpot>.Filter.Eq(s => s.UserId, userId);
            var update = Builders<ParkingSpot>.Update.Set(s => s.Availability, availability);
            var options = new FindOneAndUpdateOptions<ParkingSpot> { ReturnDocument = ReturnDocument.After };
            var updatedSpot = await _parkingSpots.FindOneAndUpdateAsync(filter, update, options);
            return updatedSpot?.Availability ?? new List<Availability>();
        }
    }
}
