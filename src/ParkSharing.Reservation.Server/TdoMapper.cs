using App.Context.Models;
using Nelibur.ObjectMapper;
using ParkSharing.Contracts;

namespace ParkSharing.Reservation.Server
{
    public static class Mapper
    {
        public static void BindMaps()
        {
            TinyMapper.Bind<ParkSpotCreatedOrUpdatedEvent, ParkingSpot>();
            TinyMapper.Bind<Availability, AvailabilityCreatedOrUpdatedEvent>();
            TinyMapper.Bind<Recurrence, RecurrenceCreatedOrUpdatedEvent>();
            TinyMapper.Bind<ReservationCreatedEvent, ReservationSpot>();
            TinyMapper.Bind<ReservationSpot, ReservationCreatedEvent>();
        }
    }
}
