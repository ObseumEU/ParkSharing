using ParkSharing.Notification.Server.Email;
using ParkSharing.Notification.Server.Services;
using System.Reflection;
using ParkSharing.Notification.Server.SMS;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var config = builder.Configuration;

builder.Services.Configure<TwilioOptions>(builder.Configuration.GetSection("Twilio"));

builder.Services.AddHttpClient("TwilioClient", client =>
{
    client.BaseAddress = new Uri("https://api.twilio.com/2010-04-01/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddTransient<SMSClient>();

builder.ConfigureMassTransit(config.GetConnectionString("rabbitmq"), Assembly.GetExecutingAssembly());
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpOptions"));

builder.Services.AddScoped<IEmailClient, EmailClient>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
// Add services to the container.

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting ParkSharing Notification Service");
logger.LogInformation("Environment: {Environment}", builder.Environment.EnvironmentName);
logger.LogInformation("Application Name: {AppName}", builder.Environment.ApplicationName);

app.Run();

