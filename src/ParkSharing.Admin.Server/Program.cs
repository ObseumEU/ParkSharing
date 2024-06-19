using App;
using App.Authorization;
using App.Consumers;
using App.Context.Models;
using App.Middlewares;
using App.Requirement;
using App.Services;
using dotenv.net;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
Mapper.BindMaps();
// Add Service Defaults
builder.AddServiceDefaults();
builder.AddMongoDBClient("mongodb");

// Register custom services
builder.Services.AddScoped<IMongoDbContext, MongoDbContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = "AdminParkSharing"; // Ensure this is configured in your settings
    return new MongoDbContext(client, databaseName);
});

builder.Services.AddScoped<DebugSeedData>(); // Register SeedData service

builder.ConfigureMassTransit(config.GetConnectionString("rabbitmq"), Assembly.GetExecutingAssembly());

// Add Configuration
builder.Host.ConfigureAppConfiguration((configBuilder) =>
{
    configBuilder.Sources.Clear();
    DotEnv.Load();
    configBuilder.AddEnvironmentVariables();
});

// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

// Add Services to the Container
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IParkingSpotService, ParkingSpotServiceMongo>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new NullableDayOfWeekConverter());
});
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .WithHeaders(new string[] {
                HeaderNames.ContentType,
                HeaderNames.Authorization,
              })
              .AllowAnyMethod()
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
    });
});
IdentityModelEventSource.ShowPII = true;

// Configure Authentication
builder.Host.ConfigureServices((services) =>
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{builder.Configuration.GetValue<string>("AUTH0_DOMAIN")}/";
            options.Audience = builder.Configuration.GetValue<string>("AUTH0_AUDIENCE");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = $"https://{builder.Configuration.GetValue<string>("AUTH0_DOMAIN")}/",
                ValidAudience = builder.Configuration.GetValue<string>("AUTH0_AUDIENCE"),
            };
        });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("read:admin-reservations", policy =>
        {
            policy.Requirements.Add(new RbacRequirement("read:admin-reservations"));
        });
    });

    services.AddSingleton<IAuthorizationHandler, RbacHandler>();
}
);

var app = builder.Build();

#if DEBUG
using (var scope = app.Services.CreateScope())
{
    var seedData = scope.ServiceProvider.GetRequiredService<DebugSeedData>();
    var bus = scope.ServiceProvider.GetRequiredService<IBusControl>();
    await bus.StartAsync();
    await bus.StopAsync();
    await seedData.InitializeAsync();
}
#endif

// Validate Configuration Variables
var requiredVars = new string[] {
    "CLIENT_ORIGIN_URL",
    "AUTH0_DOMAIN",
    "AUTH0_AUDIENCE",
};

foreach (var key in requiredVars)
{
    var value = app.Configuration.GetValue<string>(key);

    if (string.IsNullOrEmpty(value))
    {
        throw new Exception($"Config variable missing: {key}.");
    }
}

//app.Urls.Add($"http://+:{app.Configuration.GetValue<string>("PORT")}");

// Middleware Configuration
app.UseErrorHandler();
app.UseSecureHeaders();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
