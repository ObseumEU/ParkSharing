using App.Context.Models;
using MongoDB.Driver;

public interface IMongoDbContext
{
    IMongoCollection<ParkingSpot> ParkingSpots { get; }
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<ParkingSpot> ParkingSpots => _database.GetCollection<ParkingSpot>("AdminParkingSpots");
}