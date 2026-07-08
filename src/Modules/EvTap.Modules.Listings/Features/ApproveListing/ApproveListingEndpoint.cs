using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.ApproveListing;

public sealed class ApproveListingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/listings/{id:guid}/approve", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ApproveListingCommand(id));

            return result.ToHttpResult();
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}
