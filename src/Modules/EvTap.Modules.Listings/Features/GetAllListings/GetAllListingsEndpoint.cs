using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetAllListings;

public sealed class GetAllListingsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/listings/all", async (string? status, ISender sender) =>
        {
            var result = await sender.Send(new GetAllListingsQuery(status));

            return result.ToHttpResult();
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}
