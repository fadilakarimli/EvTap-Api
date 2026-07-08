using Carter;
using EvTap.Modules.Listings;
using EvTap.Modules.Listings.Persistence;
using EvTap.Modules.Notifications;
using EvTap.Modules.Notifications.Consumers;
using EvTap.Modules.Notifications.Persistence;
using EvTap.Modules.Notifications.Realtime;
using EvTap.Modules.SavedSearches;
using EvTap.Modules.SavedSearches.Persistence;
using EvTap.Modules.Users;
using EvTap.Modules.Users.Persistence;
using EvTap.Modules.Users.Security;
using EvTap.Shared.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddCarter();

    builder.Services.AddSharedPipelineBehaviors();

    builder.Services.AddUsersModule(builder.Configuration);
    builder.Services.AddListingsModule(builder.Configuration);
    builder.Services.AddSavedSearchesModule(builder.Configuration);
    builder.Services.AddNotificationsModule(builder.Configuration);

    builder.Services.AddSignalR();

    builder.Services.AddMassTransit(bus =>
    {
        bus.AddConsumer<ListingApprovedConsumer>();

        // Transactional outbox on the publishing side: ListingApprovedEvent rows are written
        // to listings-schema outbox tables in the same transaction as the Status update and
        // relayed to RabbitMQ afterwards.
        bus.AddEntityFrameworkOutbox<ListingsDbContext>(outbox =>
        {
            outbox.UsePostgres();
            outbox.UseBusOutbox();
        });

        bus.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri(builder.Configuration.GetConnectionString("rabbitmq")
                ?? throw new InvalidOperationException("RabbitMQ connection string is missing.")));

            cfg.ConfigureEndpoints(context);
        });
    });

    var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
        ?? throw new InvalidOperationException("Jwt configuration section is missing.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            };

            // SignalR WebSocket connections cannot send an Authorization header; the JS/.NET
            // clients pass the JWT as ?access_token=... instead.
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken) &&
                        context.HttpContext.Request.Path.StartsWithSegments("/hubs/notifications"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "EvTap API", Version = "v1" });

        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Enter a valid JWT bearer token",
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme,
            },
        };

        options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecurityScheme, Array.Empty<string>() },
        });
    });

    var app = builder.Build();

    app.MapDefaultEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        await UsersDbInitializer.MigrateAndSeedAsync(app.Services, app.Configuration);
        await ListingsDbInitializer.MigrateAsync(app.Services);
        await SavedSearchesDbInitializer.MigrateAsync(app.Services);
        await NotificationsDbInitializer.MigrateAsync(app.Services);
    }

    app.UseSerilogRequestLogging();

    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapCarter();

    app.MapHub<NotificationsHub>("/hubs/notifications");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "EvTap API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
