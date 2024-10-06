using App.Context.Models;
using MassTransit;
using MongoDB.Driver;
using ParkSharing.Contracts;

namespace App.Consumers
{
    public class GetUserInfoConsumer : IConsumer<GetUserInfo>
    {
        private readonly IMongoCollection<ParkingSpot> _parkingSpotsCollection;
        ILogger<GetUserInfoConsumer> _logger;
        IMongoClient mongoClient;

        public GetUserInfoConsumer(IMongoClient mongoClient, ILogger<GetUserInfoConsumer> logger)
        {
            var database = mongoClient.GetDatabase("AdminParkSharing");
            _parkingSpotsCollection = database.GetCollection<ParkingSpot>("ParkingSpots");
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetUserInfo> context)
        {
            var filter = Builders<ParkingSpot>.Filter.Eq(ps => ps.PublicId, context.Message.PublicSpotId);
            Console.WriteLine($"Filter: PublicSpotId = {context.Message.PublicSpotId}");

            var parkingSpot = await _parkingSpotsCollection.Find(filter).FirstOrDefaultAsync();
            if (parkingSpot == null)
            {
                throw new Exception($"Spot not found Id: {context.Message.PublicSpotId}");
            }
            await context.RespondAsync(new UserInfoResult
            {
                PublicSpotId = context.Message.PublicSpotId,
                UserId = parkingSpot.UserId,
                Email = parkingSpot.Email,
                Phone = parkingSpot.Phone,
                BankAccount = parkingSpot.BankAccount,
                SpotName = parkingSpot.Name
            });
        }

        private void UpdateExistingReservation(Reservation existingReservation, ReservationCreatedEvent message)
        {
            existingReservation.Start = message.Start;
            existingReservation.End = message.End;
            existingReservation.Phone = message.ClientPhone;
            existingReservation.Price = message.Price;
        }

        private void AddNewReservation(ParkingSpot parkingSpot, ReservationCreatedEvent message)
        {
            parkingSpot.Reservations.Add(new Reservation
            {
                PublicId = message.PublicId,
                Start = message.Start,
                End = message.End,
                Phone = message.ClientPhone,
                Price = message.Price,
            });
        }
    }
}
