using Moq;
using ParkSharing.Notification.Server.Email;

namespace ParkSharing.Notification.Server.Tests
{
    public class EmailServiceTests
    {
        private readonly Mock<IEmailClient> _emailClientMock;
        private readonly Mock<IEmailTemplateService> _templateServiceMock;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _emailClientMock = new Mock<IEmailClient>();
            _templateServiceMock = new Mock<IEmailTemplateService>();
            _emailService = new EmailService(_emailClientMock.Object, _templateServiceMock.Object);
        }

        [Fact]
        public async Task SendTemplatedEmailAsync_SendsEmailWithReplacedValues()
        {
            // Arrange
            var receiver = "test@example.com";
            var subject = "Test Subject";
            var templateName = "Welcome";
            var template = "<h1>Welcome, {name}!</h1><p>Thank you for joining us.</p>";
            var values = new Dictionary<string, string> { { "name", "John Doe" } };
            var expectedBody = "<h1>Welcome, John Doe!</h1><p>Thank you for joining us.</p>";

            _templateServiceMock.Setup(ts => ts.GetTemplateAsync(templateName)).ReturnsAsync(template);

            // Act
            await _emailService.SendTemplatedEmailAsync(receiver, subject, templateName, values);

            // Assert
            _emailClientMock.Verify(ec => ec.SendEmailAsync(receiver, subject, expectedBody), Times.Once);
        }

        [Fact]
        public async Task SendTemplatedEmailAsync_UsesEmptyTemplateWhenNotFound()
        {
            // Arrange
            var receiver = "test@example.com";
            var subject = "Test Subject";
            var templateName = "NonExistentTemplate";
            var template = string.Empty;
            var values = new Dictionary<string, string> { { "name", "John Doe" } };

            _templateServiceMock.Setup(ts => ts.GetTemplateAsync(templateName)).ReturnsAsync(template);

            // Act
            await _emailService.SendTemplatedEmailAsync(receiver, subject, templateName, values);

            // Assert
            _emailClientMock.Verify(ec => ec.SendEmailAsync(receiver, subject, template), Times.Once);
        }
    }
}
