using OpenAI.Extensions;
using ParkSharing.Services.ChatGPT;

var builder = WebApplication.CreateBuilder(args);

// Add Service Defaults
builder.AddServiceDefaults();
builder.AddMongoDBClient("mongodb");

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
builder.Services.AddSingleton<IReservationService, ReservationService>();
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<ChatGPTService>();
builder.Services.AddSingleton<ChatGPTCapabilities>();
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
