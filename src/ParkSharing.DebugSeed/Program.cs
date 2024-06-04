var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.ConfigureMassTransit(builder.Configuration.GetConnectionString("rabbitmq"));
builder.Services.AddScoped<DebugSeedData>(); // Register SeedData service

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seedData = scope.ServiceProvider.GetRequiredService<DebugSeedData>();
    await seedData.InitializeAsync();
}

app.MapDefaultEndpoints();


app.UseHttpsRedirection();

app.Run();

