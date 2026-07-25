using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetComments;

public sealed class GetCommentsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/listings/{id:guid}/comments", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCommentsQuery(id));

            return result.ToHttpResult();
        });
    }
}
