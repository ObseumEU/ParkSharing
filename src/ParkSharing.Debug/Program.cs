using MassTransit;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add Service Defaults
builder.AddServiceDefaults();
var config = builder.Configuration;
builder.ConfigureMassTransit(config.GetConnectionString("RabbitMQConnection"));

// Add Configuration
builder.Host.ConfigureAppConfiguration((configBuilder) =>
{
    configBuilder.AddEnvironmentVariables();
});

builder.Services.AddScoped<DebugSeedData>(); // Register SeedData service

// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

// Add Services to the Container
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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


#if DEBUG
using (var scope = app.Services.CreateScope())
{
    var seedData = scope.ServiceProvider.GetRequiredService<DebugSeedData>();
    await seedData.InitializeAsync();
}
#endif

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
