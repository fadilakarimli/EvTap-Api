using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetPendingListings;

public sealed class GetPendingListingsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/listings/pending", async (ISender sender) =>
        {
            var result = await sender.Send(new GetPendingListingsQuery());

            return result.ToHttpResult();
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}
