using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.SavedSearches.Features.GetMySavedSearches;

public sealed class GetMySavedSearchesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/saved-searches", async (HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new GetMySavedSearchesQuery(context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
