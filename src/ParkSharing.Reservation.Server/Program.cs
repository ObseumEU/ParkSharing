using MassTransit;
using MongoDB.Driver;
using OpenAI.Extensions;
using ParkSharing.Services.ChatGPT;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
ParkSharing.Reservation.Server.Mapper.BindMaps();
// Add Service Defaults
builder.AddServiceDefaults();
builder.AddMongoDBClient("mongodb");
var config = builder.Configuration;

// Register custom services
builder.Services.AddScoped<IMongoDbContext, MongoDbContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = "ReservationParkSharing"; // Ensure this is configured in your settings
    return new MongoDbContext(client, databaseName);
});

builder.Services.AddScoped<DebugSeedData>(); // Register SeedData service


builder.ConfigureMassTransit(config.GetConnectionString("rabbitmq"), Assembly.GetExecutingAssembly());

// Add Configuration
builder.Host.ConfigureAppConfiguration((configBuilder) =>
{
    configBuilder.AddEnvironmentVariables();
});

// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

// Add Services to the Container
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ChatGPTService>();
builder.Services.AddScoped<ChatGPTCapabilities>();


builder.Services.AddOpenAIService();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers();


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomCorsPolicy", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5239", "https://parking.obseum.cloud")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());
});

// Configure Session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "WhyAreYouLookingAtThat";
    options.IdleTimeout = TimeSpan.FromDays(10000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var seedData = scope.ServiceProvider.GetRequiredService<DebugSeedData>();
    var bus = scope.ServiceProvider.GetRequiredService<IBusControl>();
    await bus.StartAsync();
    var seed = new DebugSeedData(bus, scope.ServiceProvider.GetRequiredService<IReservationService>());
    await seed.InitializeAsync();
    await bus.StopAsync();
}


// Middleware Configuration
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        return Task.CompletedTask;
    });

    await next();
});

app.UseExceptionHandler("/Error");

// Configure the HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CustomCorsPolicy");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
