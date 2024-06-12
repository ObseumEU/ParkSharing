using App.Context.Models;
using MassTransit;
using MongoDB.Driver;
using System.Diagnostics;

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

            if (parkingSpot != null)
            {
                if(parkingSpot.Reservations == null)
                {
                    parkingSpot.Reservations = new List<Reservation>();
                }
                
                parkingSpot.Reservations.Add(
                    new Reservation
                    {
                        PublicId = context.Message.PublicId,
                        Start = context.Message.Start,
                        End = context.Message.End,
                        Phone = context.Message.Phone,
                        Price = context.Message.Price,
                    });
                var update = Builders<ParkingSpot>.Update.Set(ps => ps.Reservations, parkingSpot.Reservations);
                _parkingSpotsCollection.UpdateOne(filter, update);
            }
            else
            {
                throw new Exception($"Spot not found Id:{context.Message.PublicId}");
            }
        }
    }
}
