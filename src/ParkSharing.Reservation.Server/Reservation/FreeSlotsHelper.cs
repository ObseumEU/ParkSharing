using ParkSharing.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ParkSharing.Reservation.Server.Reservation
{
    public static class FreeSlotsHelper
    {
        /// <summary>
        /// Generate FreeSlot for a specific Date range.
        /// </summary>
        /// <param name="spots">List of existing spots with availability set</param>
        /// <param name="from">Generate from this date</param>
        /// <param name="to">Generate to this date</param>
        /// <returns>List of available slots</returns>
        public static List<FreeSlot> GenerateAvaliableSlots(this List<ParkingSpot> spots, DateTime from, DateTime to)
        {
            var freeSlots = new List<FreeSlot>();

            foreach (var spot in spots)
            {
                if (spot.Availability == null || spot.Availability.Count == 0)
                {
                    continue;
                }

                foreach (var availability in spot.Availability)
                {
                    switch (availability.Recurrence)
                    {
                        case AvailabilityRecurrence.Once:
                            AddOnceAvailability(freeSlots, spot, availability, from, to);
                            break;
                        case AvailabilityRecurrence.Daily:
                            AddDailyAvailability(freeSlots, spot, availability, from, to);
                            break;
                        case AvailabilityRecurrence.Weekly:
                            AddWeeklyAvailability(freeSlots, spot, availability, from, to);
                            break;
                        case AvailabilityRecurrence.WeekDays:
                            AddWeekDaysAvailability(freeSlots, spot, availability, from, to);
                            break;
                    }
                }
            }

            return MergeOverlappingSlots(freeSlots);
        }

        private static void AddOnceAvailability(List<FreeSlot> freeSlots, ParkingSpot spot, Availability availability, DateTime from, DateTime to)
        {
            if (availability.StartDate.HasValue && availability.EndDate.HasValue &&
                availability.StartDate.Value.Date <= to.Date && availability.EndDate.Value.Date >= from.Date)
            {
                if (availability.StartTime != availability.EndTime)
                {
                    AddFreeSlot(freeSlots, spot, availability.StartDate.Value.Date + availability.StartTime, availability.EndDate.Value.Date + availability.EndTime);
                }
            }
        }

        private static void AddDailyAvailability(List<FreeSlot> freeSlots, ParkingSpot spot, Availability availability, DateTime from, DateTime to)
        {
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                if (availability.StartTime != availability.EndTime)
                {
                    if (availability.EndTime <= availability.StartTime)
                    {
                        AddFreeSlot(freeSlots, spot, date + availability.StartTime, date.AddDays(1) + availability.EndTime);
                    }
                    else
                    {
                        AddFreeSlot(freeSlots, spot, date + availability.StartTime, date + availability.EndTime);
                    }
                }
            }
        }

        private static void AddWeeklyAvailability(List<FreeSlot> freeSlots, ParkingSpot spot, Availability availability, DateTime from, DateTime to)
        {
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek == availability.DayOfWeek)
                {
                    if (availability.StartTime != availability.EndTime)
                    {
                        if (availability.EndTime <= availability.StartTime)
                        {
                            AddFreeSlot(freeSlots, spot, date + availability.StartTime, date.AddDays(1) + availability.EndTime);
                        }
                        else
                        {
                            AddFreeSlot(freeSlots, spot, date + availability.StartTime, date + availability.EndTime);
                        }
                    }
                }
            }
        }

        private static void AddWeekDaysAvailability(List<FreeSlot> freeSlots, ParkingSpot spot, Availability availability, DateTime from, DateTime to)
        {
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (availability.StartTime != availability.EndTime)
                    {
                        if (availability.EndTime <= availability.StartTime)
                        {
                            AddFreeSlot(freeSlots, spot, date + availability.StartTime, date.AddDays(1) + availability.EndTime);
                        }
                        else
                        {
                            AddFreeSlot(freeSlots, spot, date + availability.StartTime, date + availability.EndTime);
                        }
                    }
                }
            }
        }

        private static void AddFreeSlot(List<FreeSlot> freeSlots, ParkingSpot spot, DateTime from, DateTime to)
        {
            if (spot.Reservations == null || spot.Reservations.Count == 0)
            {
                freeSlots.Add(new FreeSlot
                {
                    From = from,
                    To = to,
                    SpotName = spot.Name,
                    SpotPublicId = spot.PublicId,
                    PricePerHour = spot.PricePerHour
                });
            }
            else
            {
                var reservations = spot.Reservations.OrderBy(r => r.Start).ToList();
                DateTime currentStart = from;

                foreach (var reservation in reservations)
                {
                    if (reservation.Start < to && reservation.End > from)
                    {
                        if (reservation.Start > currentStart)
                        {
                            freeSlots.Add(new FreeSlot
                            {
                                From = currentStart,
                                To = reservation.Start,
                                SpotName = spot.Name,
                                SpotPublicId = spot.PublicId,
                                PricePerHour = spot.PricePerHour
                            });
                        }
                        currentStart = reservation.End > to ? to : reservation.End;
                    }
                }

                if (currentStart < to)
                {
                    freeSlots.Add(new FreeSlot
                    {
                        From = currentStart,
                        To = to,
                        SpotName = spot.Name,
                        SpotPublicId = spot.PublicId,
                        PricePerHour = spot.PricePerHour
                    });
                }
            }
        }

        private static List<FreeSlot> MergeOverlappingSlots(List<FreeSlot> slots)
        {
            if (slots == null || slots.Count == 0)
            {
                return new List<FreeSlot>();
            }

            var mergedSlots = new List<FreeSlot>();

            foreach (var slot in slots.OrderBy(s => s.From))
            {
                if (!mergedSlots.Any() || mergedSlots.Last().To < slot.From)
                {
                    mergedSlots.Add(slot);
                }
                else
                {
                    var lastSlot = mergedSlots.Last();
                    mergedSlots[mergedSlots.Count - 1] = new FreeSlot
                    {
                        From = lastSlot.From,
                        To = lastSlot.To > slot.To ? lastSlot.To : slot.To,
                        SpotName = lastSlot.SpotName,
                        SpotPublicId = lastSlot.SpotPublicId
                    };
                }
            }

            return mergedSlots;
        }
    }
}
public record FreeSlot
{
    [Required] public DateTime From { get; init; }
    [Required] public DateTime To { get; init; }
    [Required] public string SpotName { get; init; }
    [Required] public string SpotPublicId { get; init; }
    [Required] public decimal PricePerHour { get; init; }
}
