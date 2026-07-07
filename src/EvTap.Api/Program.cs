using Carter;
using EvTap.Modules.Listings;
using EvTap.Modules.Listings.Persistence;
using EvTap.Modules.Notifications;
using EvTap.Modules.SavedSearches;
using EvTap.Modules.SavedSearches.Persistence;
using EvTap.Modules.Users;
using EvTap.Modules.Users.Persistence;
using EvTap.Modules.Users.Security;
using EvTap.Shared.DependencyInjection;
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
    }

    app.UseSerilogRequestLogging();

    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapCarter();

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
