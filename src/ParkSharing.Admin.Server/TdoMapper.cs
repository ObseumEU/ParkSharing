using App.Context.Models;
using Nelibur.ObjectMapper;

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
            TinyMapper.Bind<State, StateDto>();
        }
    }
}
