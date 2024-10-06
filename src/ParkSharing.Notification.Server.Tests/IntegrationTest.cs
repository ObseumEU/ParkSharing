using App.Context.Models;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using ParkSharing.Contracts;
using ParkSharing.Notification.Server.Consumers;
using ParkSharing.Notification.Server.Email;
using ParkSharing.Notification.Server.Services;

namespace ParkSharing.Notification.Server.Tests.Integration
{
    public class NotificationConsumerTests
    {
        [Fact]
        public async Task ReservationCreatedEvent_ShouldSendEmail()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<IEmailClient, MockEmailClient>();
            services.AddScoped<IEmailTemplateService, MockEmailTemplateService>();
            services.AddScoped<IUserInfoService, MockUserInfoService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<NotificationConsumer>();

            services.AddLogging();

            var provider = services.BuildServiceProvider();

            var harness = new InMemoryTestHarness();
            var consumerHarness = harness.Consumer(() => provider.GetRequiredService<NotificationConsumer>());

            await harness.Start();

            try
            {
                var reservationEvent = new ReservationCreatedEvent
                {
                    PublicSpotId = "publicSpotId",
                    ClientPhone = "123456789",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddHours(2),
                    Price = 20
                };

                // Act
                await harness.InputQueueSendEndpoint.Send(reservationEvent);

                // Assert
                Assert.True(await harness.Consumed.Any<ReservationCreatedEvent>());
                Assert.True(await consumerHarness.Consumed.Any<ReservationCreatedEvent>());

                var emailClient = provider.GetRequiredService<IEmailClient>() as MockEmailClient;
                Assert.NotNull(emailClient);
                Assert.Single(emailClient.SentEmails);

                var sentEmail = emailClient.SentEmails[0];
                Assert.Equal("user@example.com", sentEmail.Receiver);
                Assert.Equal("Místo bylo zarezervováno", sentEmail.Subject);
                Assert.Contains("Od", sentEmail.Body);
                Assert.Contains("cena 20", sentEmail.Body);
            }
            finally
            {
                await harness.Stop();
            }
        }

        private class MockEmailClient : IEmailClient
        {
            public List<SentEmail> SentEmails { get; } = new List<SentEmail>();

            public Task SendEmailAsync(string receiver, string subject, string body)
            {
                SentEmails.Add(new SentEmail
                {
                    Receiver = receiver,
                    Subject = subject,
                    Body = body
                });
                return Task.CompletedTask;
            }

            public class SentEmail
            {
                public string Receiver { get; set; }
                public string Subject { get; set; }
                public string Body { get; set; }
            }
        }

        private class MockEmailTemplateService : IEmailTemplateService
        {
            public Task<string> GetTemplateAsync(string templateName)
            {
                return Task.FromResult("<p>Proběhla rezervace vašeho parkovacího místa. Od {start} do {end}, cena {price}. <br/> Kontakt na pronajímatele {phone}</p>");
            }
        }

        private class MockUserInfoService : IUserInfoService
        {
            public Task<UserInfoResult> GetUserInfo(string publicSpotId)
            {
                return Task.FromResult(new UserInfoResult
                {
                    Email = "user@example.com"
                });
            }
        }
    }
}
