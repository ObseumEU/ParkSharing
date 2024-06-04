using App.Context.Models;
using MassTransit;
using MongoDB.Driver;
using Nelibur.ObjectMapper;
using Polly;

public class ReservationService : IReservationService
{
    private readonly IMongoCollection<ParkingSpot> _parkingSpotsCollection;
    IBus _broker;

    public ReservationService(IMongoDbContext context, IBus broker)
    {
        _parkingSpotsCollection = context.ParkingSpots;
        _broker = broker;
    }

    public async Task<List<ParkingSpot>> GetAvailableSpotsAsync(DateTime fromUtc, DateTime toUtc)
    {
        var availabilityFilter = CreateAvailabilityFilter(fromUtc, toUtc);
        var reservationFilter = CreateReservationFilter(fromUtc, toUtc);

        var filter = Builders<ParkingSpot>.Filter.And(availabilityFilter, reservationFilter);

        return await _parkingSpotsCollection.Find(filter).ToListAsync();
    }

    public async Task<List<ReservationSpot>> GetReservationsAsync(string name, DateTime fromUtc, DateTime toUtc)
    {
        // Define the filter to match the parking spot by name
        var filter = Builders<ParkingSpot>.Filter.Eq(p => p.Name, name);

        // Define the projection to include only the reservations that do not collide with the specified time range
        var projection = Builders<ParkingSpot>.Projection
            .ElemMatch(p => p.Reservations, r => r.End <= fromUtc || r.Start >= toUtc);

        // Find the parking spot with the specified name and projection
        var parkingSpotWithReservations = await _parkingSpotsCollection
            .Find(filter)
            .Project<ParkingSpot>(projection)
            .FirstOrDefaultAsync();

        // Return the non-colliding reservations or an empty list if no reservations match
        return parkingSpotWithReservations?.Reservations ?? new List<ReservationSpot>();
    }

    public async Task<ParkingSpot> GetParkingSpotAsync(Guid parkingSpotId)
    {
        return await _parkingSpotsCollection.Find(p => p.Id == parkingSpotId.ToString()).FirstOrDefaultAsync();
    }

    public async Task<ParkingSpot> GetParkingSpotByNameAsync(string name)
    {
        return await _parkingSpotsCollection.Find(p => p.Name == name).FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveReservationAsync(Guid reservationId)
    {
        var update = Builders<ParkingSpot>.Update.PullFilter(p => p.Reservations, r => r.Id == reservationId.ToString());
        var result = await _parkingSpotsCollection.UpdateOneAsync(p => p.Reservations.Any(r => r.Id == reservationId.ToString()), update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> ReserveAsync(string spotName, ReservationSpot reservation)
    {
        ValidateReservation(reservation);

        var existingReservation = await GetAvailableSpotsAsync(reservation.Start.Value, reservation.End.Value);
        if (existingReservation?.Count == 0)
        {
            return false;
        }

        var filter = Builders<ParkingSpot>.Filter.Eq(ps => ps.Name, spotName);
        var parkingSpot = await _parkingSpotsCollection.Find(filter).FirstOrDefaultAsync();

        if(parkingSpot == null)
        {
            return false;
        }

        if (parkingSpot.Reservations == null)
        {
            parkingSpot.Reservations = new List<ReservationSpot>();
        }
        parkingSpot.Reservations.Add(reservation);
        var update = Builders<ParkingSpot>.Update.Set(ps => ps.Reservations, parkingSpot.Reservations);
        var result = await _parkingSpotsCollection.UpdateOneAsync(filter, update);

        var success = result.ModifiedCount > 0;

        if (success)
        {
            //Publish event add reservation
            await _broker.Publish(new ReservationCreatedEvent()
            {
                End = reservation.End,
                Phone = reservation.Phone,
                PublicId = reservation.PublicId,
                PublicSpotId = parkingSpot.PublicId,
                Start = reservation.Start,
                Price = reservation.Price
            });
        }
        return success;
    }

    private FilterDefinition<ParkingSpot> CreateAvailabilityFilter(DateTime fromUtc, DateTime toUtc)
    {
        TimeSpan fromTimeOfDay = fromUtc.TimeOfDay;
        TimeSpan toTimeOfDay = toUtc.TimeOfDay;
        DayOfWeek fromDayOfWeek = fromUtc.DayOfWeek;
        DayOfWeek toDayOfWeek = toUtc.DayOfWeek;

        return Builders<ParkingSpot>.Filter.ElemMatch(p => p.Availability, a =>
            (a.Recurrence == Recurrence.Daily && fromTimeOfDay >= a.Start && toTimeOfDay <= a.End) ||
            (a.Recurrence == Recurrence.Weekly && a.DayOfWeek == fromDayOfWeek && fromTimeOfDay >= a.Start && toTimeOfDay <= a.End) ||
            (a.Recurrence == Recurrence.Weekly && a.DayOfWeek == toDayOfWeek && fromTimeOfDay >= a.Start && toTimeOfDay <= a.End) ||
            (a.Recurrence == Recurrence.Monthly && fromTimeOfDay >= a.Start && toTimeOfDay <= a.End)
        );
    }

    private FilterDefinition<ParkingSpot> CreateReservationFilter(DateTime fromUtc, DateTime toUtc)
    {
        return Builders<ParkingSpot>.Filter.Not(Builders<ParkingSpot>.Filter.ElemMatch(p => p.Reservations, r =>
            r.Start < toUtc && r.End > fromUtc));
    }

    private void ValidateReservation(ReservationSpot reservation)
    {
        if (reservation.Start == null || reservation.End == null)
        {
            throw new ArgumentException("Reservation must have valid start and end times.");
        }
    }

    private bool IsSpotAvailable(ParkingSpot freeSpot, ReservationSpot reservation)
    {
        return freeSpot.Availability.Any(a =>
            (a.Recurrence == Recurrence.Daily && reservation.Start.Value.TimeOfDay >= a.Start && reservation.End.Value.TimeOfDay <= a.End) ||
            (a.Recurrence == Recurrence.Weekly && a.DayOfWeek == reservation.Start.Value.DayOfWeek && reservation.Start.Value.TimeOfDay >= a.Start && reservation.End.Value.TimeOfDay <= a.End) ||
            (a.Recurrence == Recurrence.Weekly && a.DayOfWeek == reservation.End.Value.DayOfWeek && reservation.Start.Value.TimeOfDay >= a.Start && reservation.End.Value.TimeOfDay <= a.End) ||
            (a.Recurrence == Recurrence.Monthly && reservation.Start.Value.TimeOfDay >= a.Start && reservation.End.Value.TimeOfDay <= a.End)
        );
    }
}
