using ParkSharing.Notification.Server.Email;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<IEmailClient, EmailClient>();
builder.Services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddSingleton<EmailService>();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();


app.UseHttpsRedirection();

app.Run();

