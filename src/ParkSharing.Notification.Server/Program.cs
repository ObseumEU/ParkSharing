using ParkSharing.Notification.Server.Email;
using ParkSharing.Notification.Server.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.AddServiceDefaults();
builder.ConfigureMassTransit(config.GetConnectionString("rabbitmq"), Assembly.GetExecutingAssembly());
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpOptions"));


builder.Host.ConfigureAppConfiguration((configBuilder) =>
{
    configBuilder.AddEnvironmentVariables();

});

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

