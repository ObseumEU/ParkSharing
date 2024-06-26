using MongoDB.Driver;

public interface IMongoDbContext
{
    IMongoCollection<ParkingSpot> ParkingSpots { get; }
    IMongoCollection<ParkSharing.Reservation.Server.Services.Session.Model.Session> Sessions { get; }
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<ParkingSpot> ParkingSpots => _database.GetCollection<ParkingSpot>("ReservationParkingSpots");
    public IMongoCollection<ParkSharing.Reservation.Server.Services.Session.Model.Session> Sessions => _database.GetCollection<ParkSharing.Reservation.Server.Services.Session.Model.Session>("Sessions");

}