var builder = DistributedApplication.CreateBuilder(args);

// Fixed (non-random) credentials: the data volumes below are persistent across AppHost
// restarts, but Aspire generates a fresh random password every launch by default. Postgres/
// RabbitMQ only apply that password on first initialization, so a persisted volume + a new
// random password on each run caused an auth-failure loop against the old (baked-in) password.
var postgresPassword = builder.AddParameter("postgres-password", value: "evtap-dev-postgres", secret: true);
var rabbitmqPassword = builder.AddParameter("rabbitmq-password", value: "evtap-dev-rabbitmq", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var evtapDb = postgres.AddDatabase("evtapdb");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("rabbitmq", password: rabbitmqPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

builder.AddProject<Projects.EvTap_Api>("api")
    .WithReference(evtapDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(evtapDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    // Fixed host port (instead of Aspire's default random port) so the frontend's API base URL
    // doesn't need to change on every AppHost restart.
    .WithHttpEndpoint(port: 5080, name: "http");

builder.Build().Run();
