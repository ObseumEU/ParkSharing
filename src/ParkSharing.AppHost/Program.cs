using ParkSharing.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB(ServicesNames.Mongo, 2023);
var mongodb = mongo.AddDatabase(ServicesNames.MongoDb);

var messaging = builder.AddRabbitMQ(ServicesNames.Rabbit);


var reservation = builder.AddProject<Projects.ParkSharing_Reservation_Server>(ServicesNames.ReservationServer)
    .WithReference(mongodb)
    .WithReference(messaging)
    .WithExternalHttpEndpoints();

var admin = builder.AddProject<Projects.ParkSharing_Admin_Server>(ServicesNames.AdminServer)
    .WithReference(mongodb)
    .WithReference(messaging)
    .WithExternalHttpEndpoints();

builder.AddNpmApp(ServicesNames.ReservationClient, "../ParkSharing.Reservation.Client")
    .WithReference(reservation)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddNpmApp(ServicesNames.AdminClient, "../ParkSharing.Admin.Client")
    .WithReference(admin)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();