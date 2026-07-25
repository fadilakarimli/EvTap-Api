using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.RestoreListing;

public sealed class RestoreListingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/listings/{id:guid}/restore", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new RestoreListingCommand(id));

            return result.ToHttpResult();
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}
