using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.GetListings;

public sealed class GetListingsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/listings", async (
            decimal? minPrice,
            decimal? maxPrice,
            int? rooms,
            string? district,
            string? sortBy,
            int page,
            int pageSize,
            ISender sender) =>
        {
            var query = new GetListingsQuery(
                minPrice,
                maxPrice,
                rooms,
                district,
                sortBy,
                page == 0 ? 1 : page,
                pageSize == 0 ? 20 : pageSize);

            var result = await sender.Send(query);

            return result.ToHttpResult();
        });
    }
}
