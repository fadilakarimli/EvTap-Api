using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetListingById;

public sealed class GetListingByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/listings/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetListingByIdQuery(id));

            return result.ToHttpResult();
        });
    }
}
