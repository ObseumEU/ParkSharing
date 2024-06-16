using ParkSharing.AppHost;
using static System.Net.WebRequestMethods;

var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB(ServicesNames.Mongo, 2023);
var mongodb = mongo.AddDatabase(ServicesNames.MongoDb);

var messaging = builder.AddRabbitMQ(ServicesNames.Rabbit);

var reservation = builder.AddProject<Projects.ParkSharing_Reservation_Server>(ServicesNames.ReservationServer)
    .WithReference(mongodb)
    .WithReference(messaging)
    //.WithHttpEndpoint(env: "PORT", port: 4222)
    .WithExternalHttpEndpoints();

var admin = builder.AddProject<Projects.ParkSharing_Admin_Server>(ServicesNames.AdminServer)
    .WithReference(mongodb)
    .WithReference(messaging)
    .WithEnvironment("CLIENT_ORIGIN_URL", "localhost")
    .WithEnvironment("AUTH0_AUDIENCE", "https://parksharing.obseum.cloud")
    .WithEnvironment("AUTH0_DOMAIN", "dev-j8mvyoxwsvvvkvs6.eu.auth0.com")
    //.WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithExternalHttpEndpoints();

builder.AddNpmApp(ServicesNames.ReservationClient, "../ParkSharing.Reservation.Client")
    .WithReference(reservation)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "PORT", port: 4224)
    .WithEnvironment("REACT_APP_API_SERVER_URL", "http://localhost:5084")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var ss = admin.GetEndpoint("http");
builder.AddNpmApp(ServicesNames.AdminClient, "../ParkSharing.Admin.Client")
    .WithReference(admin)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("REACT_APP_AUTH0_DOMAIN", "dev-j8mvyoxwsvvvkvs6.eu.auth0.com")
    .WithEnvironment("REACT_APP_AUTH0_CLIENT_ID", "b7ZPBulSPQXCzgolfNAWkxERDPGDTZoz")
    .WithEnvironment("REACT_APP_AUTH0_CALLBACK_URL", "http://localhost:4225/callback")
    .WithEnvironment("REACT_APP_API_SERVER_URL", "http://localhost:4355")

    .WithEnvironment("REACT_APP_AUTH0_AUDIENCE", "https://parksharing.obseum.cloud")
    .WithHttpEndpoint(env: "PORT", port: 4225)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();