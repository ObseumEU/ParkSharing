using ParkSharing.Contracts;
using ParkSharing.Reservation.Server.Reservation;

namespace ParkSharing.Reservation.Server.Tests
{
    public class FreeSlotsHelperTests
    {
        private List<ParkingSpot> CreateParkingSpots(string publicId, string name, List<Availability> availabilities)
        {
            return new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = publicId,
                    Name = name,
                    Availability = availabilities
                }
            };
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnExactFreeSlot_ForSingleDayExactMatch()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 9, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 6, 9, 8, 0, 0), result[0].From);
            Assert.Equal(new DateTime(2024, 6, 9, 18, 0, 0), result[0].To);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlot_ForSingleDayPartialMatch()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 10, 0, 0);
            var to = new DateTime(2024, 6, 9, 12, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 6, 9, 8, 0, 0), result[0].From);
            Assert.Equal(new DateTime(2024, 6, 9, 18, 0, 0), result[0].To);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlot_ForSingleDayOutsideRange()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 6, 0, 0);
            var to = new DateTime(2024, 6, 9, 20, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 6, 9, 8, 0, 0), result[0].From);
            Assert.Equal(new DateTime(2024, 6, 9, 18, 0, 0), result[0].To);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlots_ForTwoDaysExactMatch()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 10, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 18, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 8, 0, 0) && fs.To == new DateTime(2024, 6, 10, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnMergedFreeSlots_ForOverlappingAvailabilities()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                },
                new Availability
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(21, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 10, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 21, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 8, 0, 0) && fs.To == new DateTime(2024, 6, 10, 21, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlot_ForWeeklyAvailability()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Weekly,
                    DayOfWeek = DayOfWeek.Monday
                }
            });

            var from = new DateTime(2024, 6, 10, 5, 0, 0);
            var to = new DateTime(2024, 6, 10, 16, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 8, 0, 0) && fs.To == new DateTime(2024, 6, 10, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnEmpty_ForNonMatchingWeeklyAvailability()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Weekly,
                    DayOfWeek = DayOfWeek.Monday
                }
            });

            var from = new DateTime(2024, 6, 9, 5, 0, 0); // Sunday
            var to = new DateTime(2024, 6, 9, 16, 0, 0); // Sunday

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlot_ForWeekdaysAvailability()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.WeekDays
                }
            });

            var from = new DateTime(2024, 6, 10, 5, 0, 0); // Monday
            var to = new DateTime(2024, 6, 10, 16, 0, 0); // Monday

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 8, 0, 0) && fs.To == new DateTime(2024, 6, 10, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnEmpty_ForNonMatchingWeekdaysAvailability()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.WeekDays
                }
            });

            var from = new DateTime(2024, 6, 9, 5, 0, 0); // Sunday
            var to = new DateTime(2024, 6, 9, 16, 0, 0); // Sunday

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlot_ForOnceTimeRange()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0), // In search StartTime and StartDate is combined into one. StartDate time of day
                    EndTime = new TimeSpan(18, 0, 0),
                    StartDate = new DateTime(2024, 6, 9),
                    EndDate = new DateTime(2024, 6, 13),
                    Recurrence = AvailabilityRecurrence.Once
                }
            });

            var from = new DateTime(2024, 6, 7, 5, 0, 0);
            var to = new DateTime(2024, 6, 9, 16, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 13, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldHandleOverlappingAvailabilitiesAcrossDays()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(22, 0, 0),
                    EndTime = new TimeSpan(2, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 20, 0, 0);
            var to = new DateTime(2024, 6, 10, 4, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 22, 0, 0) && fs.To == new DateTime(2024, 6, 10, 2, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 22, 0, 0) && fs.To == new DateTime(2024, 6, 11, 2, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldHandleMixedRecurrenceTypes()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                },
                new Availability
                {
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Weekly,
                    DayOfWeek = DayOfWeek.Monday
                }
            });

            var from = new DateTime(2024, 6, 10, 7, 0, 0); // Monday
            var to = new DateTime(2024, 6, 10, 19, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 8, 0, 0) && fs.To == new DateTime(2024, 6, 10, 12, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 14, 0, 0) && fs.To == new DateTime(2024, 6, 10, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldHandleZeroLengthAvailability()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Recurrence = AvailabilityRecurrence.Daily
                }
            });

            var from = new DateTime(2024, 6, 9, 9, 0, 0);
            var to = new DateTime(2024, 6, 9, 11, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlots_ForWeekends()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", new List<Availability>
            {
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Weekly,
                    DayOfWeek = DayOfWeek.Saturday
                },
                new Availability
                {
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Recurrence = AvailabilityRecurrence.Weekly,
                    DayOfWeek = DayOfWeek.Sunday
                }
            });

            var from = new DateTime(2024, 6, 8, 7, 0, 0); // Saturday
            var to = new DateTime(2024, 6, 9, 19, 0, 0); // Sunday

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 8, 8, 0, 0) && fs.To == new DateTime(2024, 6, 8, 18, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnEmpty_ForEmptyAvailability()
        {
            // Arrange
            var spots = CreateParkingSpots("spot1", "Spot 1", null);

            var from = new DateTime(2024, 6, 8, 7, 0, 0); // Saturday
            var to = new DateTime(2024, 6, 9, 19, 0, 0); // Sunday

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldExcludeReservedSlots_WithOneExistingReservation()
        {
            var spots = new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = "spot1",
                    Name = "Spot 1",
                    Availability = new List<Availability>
                    {
                        new Availability
                        {
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            Recurrence = AvailabilityRecurrence.Daily
                        }
                    },
                    Reservations = new List<ReservationSpot>
                    {
                        new ReservationSpot()
                        {
                            Start = new DateTime(2024, 6, 9, 10, 0, 0),
                            End = new DateTime(2024, 6, 9, 12, 0, 0)
                        }
                    }
                }
            };

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 9, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 10, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 12, 0, 0) && fs.To == new DateTime(2024, 6, 9, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldExcludeReservedSlots_WithMultipleNonOverlappingReservations()
        {
            var spots = new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = "spot1",
                    Name = "Spot 1",
                    Availability = new List<Availability>
                    {
                        new Availability
                        {
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            Recurrence = AvailabilityRecurrence.Daily
                        }
                    },
                    Reservations = new List<ReservationSpot>
                    {
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 10, 0, 0),
                            End = new DateTime(2024, 6, 9, 12, 0, 0)
                        },
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 14, 0, 0),
                            End = new DateTime(2024, 6, 9, 16, 0, 0)
                        }
                    }
                }
            };

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 9, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 10, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 12, 0, 0) && fs.To == new DateTime(2024, 6, 9, 14, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 16, 0, 0) && fs.To == new DateTime(2024, 6, 9, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldExcludeReservedSlots_WithOverlappingReservations()
        {
            var spots = new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = "spot1",
                    Name = "Spot 1",
                    Availability = new List<Availability>
                    {
                        new Availability
                        {
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            Recurrence = AvailabilityRecurrence.Daily
                        }
                    },
                    Reservations = new List<ReservationSpot>
                    {
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 10, 0, 0),
                            End = new DateTime(2024, 6, 9, 12, 0, 0)
                        },
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 11, 0, 0),
                            End = new DateTime(2024, 6, 9, 13, 0, 0)
                        }
                    }
                }
            };

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 9, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 10, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 13, 0, 0) && fs.To == new DateTime(2024, 6, 9, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldExcludeReservedSlots_WithReservationsAcrossDays()
        {
            var spots = new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = "spot1",
                    Name = "Spot 1",
                    Availability = new List<Availability>
                    {
                        new Availability
                        {
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            Recurrence = AvailabilityRecurrence.Daily
                        }
                    },
                    Reservations = new List<ReservationSpot>
                    {
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 17, 0, 0),
                            End = new DateTime(2024, 6, 10, 9, 0, 0)
                        }
                    }
                }
            };

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 10, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 8, 0, 0) && fs.To == new DateTime(2024, 6, 9, 17, 0, 0));
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 10, 9, 0, 0) && fs.To == new DateTime(2024, 6, 10, 18, 0, 0));
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnEmpty_WhenFullyOccupiedByReservations()
        {
            var spots = new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = "spot1",
                    Name = "Spot 1",
                    Availability = new List<Availability>
                    {
                        new Availability
                        {
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            Recurrence = AvailabilityRecurrence.Daily
                        }
                    },
                    Reservations = new List<ReservationSpot>
                    {
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 8, 0, 0),
                            End = new DateTime(2024, 6, 9, 18, 0, 0)
                        }
                    }
                }
            };

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 9, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateAvaliableSlots_ShouldReturnFreeSlot_WhenReservationStartsBeforeAvailability()
        {
            var spots = new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    PublicId = "spot1",
                    Name = "Spot 1",
                    Availability = new List<Availability>
                    {
                        new Availability
                        {
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            Recurrence = AvailabilityRecurrence.Daily
                        }
                    },
                    Reservations = new List<ReservationSpot>
                    {
                        new ReservationSpot
                        {
                            Start = new DateTime(2024, 6, 9, 7, 0, 0),
                            End = new DateTime(2024, 6, 9, 10, 0, 0)
                        }
                    }
                }
            };

            var from = new DateTime(2024, 6, 9, 8, 0, 0);
            var to = new DateTime(2024, 6, 9, 18, 0, 0);

            // Act
            var result = FreeSlotsHelper.GenerateAvaliableSlots(spots, from, to);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, fs => fs.From == new DateTime(2024, 6, 9, 10, 0, 0) && fs.To == new DateTime(2024, 6, 9, 18, 0, 0));
        }
    }
}
