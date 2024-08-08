﻿using Amazon.Runtime.Internal.Util;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ParkSharing.Services.ChatGPT;
using System.Globalization;
using System.Text.Json;

namespace ParkSharing.Reservation.Server.Tests
{
    public class ChatGPTCapabilitiesTests
    {
        private readonly Mock<IReservationService> _mockReservationService;
        private readonly Mock<IBus> _mockMessageBroker;
        private readonly ChatGPTCapabilities _chatGPTCapabilities;
        private readonly Mock<ILogger<ChatGPTCapabilities>> _mockIlogger;

        public ChatGPTCapabilitiesTests()
        {
            _mockReservationService = new Mock<IReservationService>();
            _mockMessageBroker = new Mock<IBus>();
            _mockIlogger = new Mock<ILogger<ChatGPTCapabilities>>();

            _chatGPTCapabilities = new ChatGPTCapabilities(_mockReservationService.Object, _mockMessageBroker.Object, _mockIlogger.Object);
        }

        [Fact]
        public async Task ReserveSpot_InvalidFromDateFormat_ReturnsErrorMessage()
        {
            var result = await _chatGPTCapabilities.ReserveSpot("invalid-date", "2023-01-01 12:00", "TestSpot", "123456789");

            Assert.Equal("Invalid 'from' date format.", result);
        }

        [Fact]
        public async Task ReserveSpot_InvalidToDateFormat_ReturnsErrorMessage()
        {
            var result = await _chatGPTCapabilities.ReserveSpot("2023-01-01 10:00", "invalid-date", "TestSpot", "123456789");

            Assert.Equal("Invalid 'to' date format.", result);
        }

        [Fact]
        public async Task ReserveSpot_SuccessfulReservation_ReturnsSuccessMessage()
        {
            DateTime date = DateTime.Now.AddDays(2);
            // Arrange
            var spotName = "TestSpot";
            var from = date.ToString("yyyy-MM-dd") + " 10:00";
            var to = date.ToString("yyyy-MM-dd") + " 12:00";
            var phone = "123456789";

            _mockReservationService.Setup(r => r.GetParkingSpotByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new ParkingSpot { Name = spotName, PricePerHour = 10, BankAccount = "TestBankAccount", Phone = "776234234" });

            _mockReservationService.Setup(r => r.ReserveAsync(It.IsAny<string>(), It.IsAny<ReservationSpot>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var expectedMessage = $"Reservation created TotalPrice:20 BankAccount To pay:TestBankAccount Owner Phone:776234234";

            // Act
            var result = await _chatGPTCapabilities.ReserveSpot(from, to, spotName, phone);

            // Assert
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public async Task ReserveSpot_ReservationFails_ReturnsFailureMessage()
        {
            // Arrange
            var spotName = "TestSpot";
            var from = "2023-01-01 10:00";
            var to = "2023-01-01 12:00";
            var phone = "123456789";

            _mockReservationService.Setup(r => r.GetParkingSpotByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new ParkingSpot { Name = spotName, PricePerHour = 10, BankAccount = "TestBankAccount" });

            _mockReservationService.Setup(r => r.ReserveAsync(It.IsAny<string>(), It.IsAny<ReservationSpot>(), It.IsAny<bool>()))
                .ReturnsAsync(false);

            var expectedMessage = $"Reservation not created, spot is already reserved for this time.";

            // Act
            var result = await _chatGPTCapabilities.ReserveSpot(from, to, spotName, phone);

            // Assert
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public async Task GetAllOpenSlots_InvalidFromDateFormat_ReturnsErrorMessage()
        {
            var result = await _chatGPTCapabilities.GetAllOpenSlots("invalid-date", "2023-01-01 12:00");

            Assert.Equal("Invalid 'from' date format.", result);
        }

        [Fact]
        public async Task GetAllOpenSlots_InvalidToDateFormat_ReturnsErrorMessage()
        {
            var result = await _chatGPTCapabilities.GetAllOpenSlots("2023-01-01 10:00", "invalid-date");

            Assert.Equal("Invalid 'to' date format.", result);
        }

        //[Fact]
        //public async Task GetAllOpenSlots_SuccessfulRetrieval_ReturnsFormattedSlots()
        //{
        //    DateTime date = DateTime.Now.AddDays(2);
        //    // Arrange
        //    var from = date.ToString("yyyy-MM-dd") + " 7:00";
        //    var to = date.ToString("yyyy-MM-dd") + " 19:00";

        //    _mockReservationService.Setup(r => r.GetAllOpenSlots(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
        //        .ReturnsAsync(new[]
        //        {
        //        new FreeSlot {
        //            From = ParseDateTime(date.ToString("yyyy-MM-dd") + " 9:00"),
        //            To = ParseDateTime(date.ToString("yyyy-MM-dd") + " 11:00"),
        //            SpotName = "Spot1",
        //            PricePerHour = 10 },
        //        new FreeSlot {
        //            From = ParseDateTime(date.ToString("yyyy-MM-dd") + " 11:00"),
        //            To = ParseDateTime(date.ToString("yyyy-MM-dd") + " 13:00"),
        //            SpotName = "Spot2",
        //            PricePerHour = 20 }
        //        }.ToList());

        //    var expectedMessage = "01 led 2023 09:00-01 led 2023 11:00,Spot1,PricePerHour:10:\n01 led 2023 11:00-01 led 2023 13:00,Spot2,PricePerHour:20:";

        //    // Act
        //    var result = await _chatGPTCapabilities.GetAllOpenSlots(from, to);

        //    // Assert
        //    Assert.Equal(expectedMessage, result);
        //}


        private DateTime ParseDateTime(string input)
        {
            string[] formats = { "yyyy-MM-dd H:mm", "yyyy-MM-dd HH:mm" };
            DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            return dateTime;
        }

        [Fact]
        public async Task SpotDetail_InvalidSpotIdentifier_ReturnsErrorMessage()
        {
            var result = await _chatGPTCapabilities.SpotDetail("invalid-spot-id");

            var expectedMessage = JsonSerializer.Serialize(new { error = "Invalid spot identifier." });

            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public async Task SpotDetail_ParkingSpotNotFound_ReturnsErrorMessage()
        {
            // Arrange
            var spot = Guid.NewGuid().ToString();

            _mockReservationService.Setup(r => r.GetParkingSpotAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ParkingSpot)null);

            var expectedMessage = JsonSerializer.Serialize(new { error = "Parking spot not found." });

            // Act
            var result = await _chatGPTCapabilities.SpotDetail(spot);

            // Assert
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public async Task SpotDetail_ParkingSpotFound_ReturnsSpotDetails()
        {
            // Arrange
            var spot = Guid.NewGuid();
            var parkingSpot = new ParkingSpot { Name = "Spot1", PricePerHour = 10 };

            _mockReservationService.Setup(r => r.GetParkingSpotAsync(It.IsAny<Guid>()))
                .ReturnsAsync(parkingSpot);

            var expectedMessage = JsonSerializer.Serialize(new { Name = parkingSpot.Name, PricePerHour = parkingSpot.PricePerHour });

            // Act
            var result = await _chatGPTCapabilities.SpotDetail(spot.ToString());

            // Assert
            Assert.Equal(expectedMessage, result);
        }

        [Theory]
        [InlineData("724 676 829", true)]
        [InlineData("724676829", true)]
        [InlineData("+420 724 676 829", true)]
        [InlineData("+420724676829", true)]
        [InlineData("+421 724 676 829", true)]
        [InlineData("+421724676829", true)]
        [InlineData("724 676 829 ", true)] // Trailing space
        [InlineData("7246768290", false)]   // Extra digit
        [InlineData("724 676829", true)]   // Missing space
        [InlineData("724676 829", true)]   // Missing space
        [InlineData("+422 724 676 829", false)] // Invalid country code
        [InlineData("123 456 789", true)] // Invalid phone number
        public void IsValidPhoneNumber_ShouldValidateCorrectly(string phoneNumber, bool expected)
        {
            // Arrange & Act
            var result = ChatGPTCapabilities.IsValidPhoneNumber(phoneNumber);

            // Assert
            result.Should().Be(expected, $"because the phone number being tested is: {phoneNumber}");
        }


    }
}