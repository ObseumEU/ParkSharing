using App.Context.Models;
using MassTransit;
using MongoDB.Driver;

namespace App.Consumers
{
    public class ReservationConsumer : IConsumer<ReservationCreatedEvent>
    {
        private readonly IMongoCollection<ParkingSpot> _parkingSpotsCollection;
        ILogger<ReservationConsumer> _logger;
        IMongoClient mongoClient;

        public ReservationConsumer(IMongoClient mongoClient, ILogger<ReservationConsumer> logger)
        {
            var database = mongoClient.GetDatabase("AdminParkSharing");
            _parkingSpotsCollection = database.GetCollection<ParkingSpot>("ParkingSpots");
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReservationCreatedEvent> context)
        {
            var filter = Builders<ParkingSpot>.Filter.Eq(ps => ps.PublicId, context.Message.PublicSpotId);
            Console.WriteLine($"Filter: PublicSpotId = {context.Message.PublicSpotId}");

            var parkingSpot = await _parkingSpotsCollection.Find(filter).FirstOrDefaultAsync();
            if (parkingSpot == null)
            {
                throw new Exception($"Spot not found Id: {context.Message.PublicSpotId}");
            }

            if (parkingSpot.Reservations == null)
            {
                parkingSpot.Reservations = new List<Reservation>();
            }

            var existingReservation = parkingSpot.Reservations
                                                 .FirstOrDefault(r => r.PublicId == context.Message.PublicId);

            if (existingReservation != null)
            {
                UpdateExistingReservation(existingReservation, context.Message);
            }
            else
            {
                AddNewReservation(parkingSpot, context.Message);
            }

            var update = Builders<ParkingSpot>.Update.Set(ps => ps.Reservations, parkingSpot.Reservations);
            await _parkingSpotsCollection.UpdateOneAsync(filter, update);
        }

        private void UpdateExistingReservation(Reservation existingReservation, ReservationCreatedEvent message)
        {
            existingReservation.Start = message.Start;
            existingReservation.End = message.End;
            existingReservation.Phone = message.Phone;
            existingReservation.Price = message.Price;
        }

        private void AddNewReservation(ParkingSpot parkingSpot, ReservationCreatedEvent message)
        {
            parkingSpot.Reservations.Add(new Reservation
            {
                PublicId = message.PublicId,
                Start = message.Start,
                End = message.End,
                Phone = message.Phone,
                Price = message.Price,
            });
        }
    }
}
