using Microsoft.AspNetCore.Http;
using OpenAI.Extensions;
using ParkingReservationApp.Services.ChatGPT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IReservationService, ReservationService>();

builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<ChatGPTService>();
builder.Services.AddSingleton<ChatGPTCapabilities>();
builder.Services.AddOpenAIService();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomCorsPolicy", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5239", "https://parking.obseum.cloud")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "WhyAreYouLookingAtThat";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllers();

var app = builder.Build();

//I dont wnat to say whole worl its ASP.NET
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("Server");  // Remove the Server header
        context.Response.Headers.Remove("X-Powered-By");  // Remove the X-Powered-By header
        return Task.CompletedTask;
    });

    await next();
});
app.UseExceptionHandler("/Error");  // Redirect to usless error page.


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Ensure requests are redirected to HTTPS

app.UseCors("CustomCorsPolicy");
app.UseStaticFiles();

app.UseRouting(); // Explicitly define UseRouting

app.UseSession(); // Must be after UseRouting but before UseEndpoints

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
