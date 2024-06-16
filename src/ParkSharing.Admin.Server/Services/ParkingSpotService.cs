using App.Context.Models;
using MassTransit;
using MongoDB.Driver;
using Nelibur.ObjectMapper;
using ParkSharing.Contracts;

namespace App.Services
{
    public interface IParkingSpotService
    {
        Task<ParkingSpot> GetSpotByUser(string userId);
        Task<List<Availability>> GetAvailabilityByUser(string userId);
        Task UpdateSpot(ParkingSpot spot);
        Task<List<Availability>> UpdateAvailabilityByUser(string userId, List<Availability> availability);
        Task InsertSpot(ParkingSpot spot);
        Task RemoveReservation(string reservationPublicId);
    }

    public class ParkingSpotServiceMongo : IParkingSpotService
    {
        private readonly IMongoCollection<ParkingSpot> _parkingSpots;
        IBus _messageBroker;

        public ParkingSpotServiceMongo(IMongoClient mongoClient, IBus messageBroker )
        {
            var database = mongoClient.GetDatabase("AdminParkSharing");
            _parkingSpots = database.GetCollection<ParkingSpot>("ParkingSpots");
            _messageBroker = messageBroker;
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

        public async Task InsertSpot(ParkingSpot spot)
        {
            if (string.IsNullOrEmpty(spot.PublicId))
            {
                spot.PublicId = Guid.NewGuid().ToString();
            }

            await _parkingSpots.InsertOneAsync(spot);

            //Emit event
            await _messageBroker.Publish(TinyMapper.Map<ParkSpotCreatedOrUpdatedEvent>(spot));
        }

        public async Task<List<Availability>> UpdateAvailabilityByUser(string userId, List<Availability> availability)
        {
            var filter = Builders<ParkingSpot>.Filter.Eq(s => s.UserId, userId);
            var update = Builders<ParkingSpot>.Update.Set(s => s.Availability, availability);
            var options = new FindOneAndUpdateOptions<ParkingSpot> { ReturnDocument = ReturnDocument.After };
            var updatedSpot = await _parkingSpots.FindOneAndUpdateAsync(filter, update, options);
            return updatedSpot?.Availability ?? new List<Availability>();
        }

        public async Task RemoveReservation(string reservationPublicId)
        {
            // Find the parking spot that contains the reservation with the given PublicId
            var filter = Builders<ParkingSpot>.Filter.ElemMatch(spot => spot.Reservations, reservation => reservation.PublicId == reservationPublicId);
            var update = Builders<ParkingSpot>.Update.PullFilter(spot => spot.Reservations, reservation => reservation.PublicId == reservationPublicId);

            // Apply the update
            var result = await _parkingSpots.UpdateOneAsync(filter, update);

            //Emit event
            await _messageBroker.Publish(new ReservationRemovedEvent()
            {
                PublicId = reservationPublicId
            });

            // Check if the update was successful
            if (result.ModifiedCount == 0)
            {
                throw new Exception("Reservation not found or removal failed.");
            }
        }
    }
}
