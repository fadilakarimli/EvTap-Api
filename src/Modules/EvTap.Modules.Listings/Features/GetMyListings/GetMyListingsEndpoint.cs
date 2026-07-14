using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetMyListings;

public sealed class GetMyListingsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/listings/mine", async (HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new GetMyListingsQuery(context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
