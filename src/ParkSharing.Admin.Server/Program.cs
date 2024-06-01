using App.Middlewares;
using App.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add Service Defaults
builder.AddServiceDefaults();
builder.AddMongoDBClient("mongodb");

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
builder.Services.AddControllers();

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

// Configure Authentication
builder.Host.ConfigureServices((services) =>
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var audience = builder.Configuration.GetValue<string>("AUTH0_AUDIENCE");
            options.Authority = $"https://{builder.Configuration.GetValue<string>("AUTH0_DOMAIN")}/";
            options.Audience = audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuerSigningKey = true
            };
        })
);

var app = builder.Build();

// Validate Configuration Variables
var requiredVars = new string[] {
    "PORT",
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

app.Urls.Add($"http://+:{app.Configuration.GetValue<string>("PORT")}");

// Middleware Configuration
app.UseErrorHandler();
app.UseSecureHeaders();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
