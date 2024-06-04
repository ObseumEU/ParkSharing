using App.Context.Models;
using Nelibur.ObjectMapper;
using ParkSharing.Contracts;

namespace App
{
    public static class Mapper
    {
        public static void BindMaps()
        {
            TinyMapper.Bind<ParkingSpot, ParkingSpotDto>();
            TinyMapper.Bind<Availability, AvailabilityDto>();
            TinyMapper.Bind<AvailabilityDto, Availability>();
            TinyMapper.Bind<Reservation, ReservationDto>();
            TinyMapper.Bind<ParkingSpot, ParkSpotCreatedOrUpdatedEvent>();
            TinyMapper.Bind<Availability, AvailabilityCreatedOrUpdatedEvent>();
            TinyMapper.Bind<State, StateDto>();
        }
    }
}
