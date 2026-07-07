using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.RemoveFavorite;

public sealed class RemoveFavoriteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/listings/{id:guid}/favorite", async (Guid id, HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new RemoveFavoriteCommand(id, context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
