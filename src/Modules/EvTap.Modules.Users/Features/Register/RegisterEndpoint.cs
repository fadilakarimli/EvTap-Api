using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Users.Features.Register;

public sealed class RegisterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/register", async (RegisterRequest request, ISender sender) =>
        {
            var command = new RegisterCommand(request.Name, request.Email, request.Password);
            var result = await sender.Send(command);

            return result.ToCreatedResult(id => $"/api/users/{id}");
        });
    }
}

public sealed record RegisterRequest(string Name, string Email, string Password);
