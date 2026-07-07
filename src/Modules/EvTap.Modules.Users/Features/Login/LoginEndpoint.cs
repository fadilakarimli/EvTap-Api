using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Users.Features.Login;

public sealed class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/login", async (LoginRequest request, ISender sender) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command);

            return result.ToHttpResult();
        });
    }
}

public sealed record LoginRequest(string Email, string Password);
