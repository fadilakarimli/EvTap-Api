using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetMyFavorites;

public sealed class GetMyFavoritesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/favorites", async (HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new GetMyFavoritesQuery(context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
