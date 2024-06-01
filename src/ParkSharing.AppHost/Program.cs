var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo");
var mongodb = mongo.AddDatabase("mongodb");

var admin = builder.AddProject<Projects.ParkSharing_Admin_Server>("admin-server")
    .WithReference(mongodb)
    .WithExternalHttpEndpoints();

var reservation = builder.AddProject<Projects.ParkSharing_Reservation_Server>("reservation-server")
    .WithReference(mongodb)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("reservation-client", "../ParkSharing.Reservation.Client")
    .WithReference(reservation)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddNpmApp("admin-client", "../ParkSharing.Admin.Client")
    .WithReference(admin)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();