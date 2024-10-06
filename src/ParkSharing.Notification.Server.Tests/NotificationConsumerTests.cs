using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using ParkSharing.Contracts;
using ParkSharing.Notification.Server.Consumers;
using ParkSharing.Notification.Server.Email;
using ParkSharing.Notification.Server.SMS;
using ParkSharing.Notification.Server.Services;
using App.Context.Models;
using ParkSharing.Notification.Server;

public class NotificationConsumerTests
{
    private readonly Mock<ILogger<NotificationConsumer>> _loggerMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUserInfoService> _userInfoServiceMock;
    private readonly Mock<SMSClient> _smsClientMock;
    private readonly Mock<IFeatureManager> _featureManagerMock;
    private readonly NotificationConsumer _consumer;

    public NotificationConsumerTests()
    {
        _loggerMock = new Mock<ILogger<NotificationConsumer>>();
        _emailServiceMock = new Mock<IEmailService>();
        _userInfoServiceMock = new Mock<IUserInfoService>();
        _smsClientMock = new Mock<SMSClient>();
        _featureManagerMock = new Mock<IFeatureManager>();

        _consumer = new NotificationConsumer(
            _loggerMock.Object,
            _emailServiceMock.Object,
            _userInfoServiceMock.Object,
            _smsClientMock.Object,
            _featureManagerMock.Object
        );
    }

    [Fact]
    public async Task Consume_ShouldSendEmailAndSms_WhenNotificationsEnabled()
    {
        // Arrange
        var reservationCreatedEvent = new ReservationCreatedEvent
        {
            PublicSpotId = "spot123",
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(2),
            ClientPhone = "123456789",
            Price = 200
        };

        var userInfo = new UserInfoResult
        {
            Email = "user@example.com",
            PublicSpotId = "spot123",
            UserId = "user123",
            Phone = "987654321",
            BankAccount = "123-456-789",
            SpotName = "Spot1"
        };

        _userInfoServiceMock
            .Setup(x => x.GetUserInfo(It.IsAny<string>()))
            .ReturnsAsync(userInfo);

        _featureManagerMock
            .Setup(x => x.IsEnabledAsync(FeatureFlags.EmailNotifications))
            .ReturnsAsync(true);

        _featureManagerMock
            .Setup(x => x.IsEnabledAsync(FeatureFlags.SMSNotifications))
            .ReturnsAsync(true);

        var consumeContextMock = new Mock<ConsumeContext<ReservationCreatedEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(reservationCreatedEvent);

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        _emailServiceMock.Verify(x => x.SendTemplatedEmailAsync(
            userInfo.Email,
            "Místo bylo zarezervováno",
            "Reservation",
            It.IsAny<Dictionary<string, string>>()), Times.Once);

        _smsClientMock.Verify(x => x.SendSmsAsync(
            reservationCreatedEvent.ClientPhone,
            It.Is<string>(s => s.Contains("Vaše rezervace místa"))), Times.Once);
    }
}
