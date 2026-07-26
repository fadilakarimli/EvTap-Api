using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.UpdateListing;

public sealed class UpdateListingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/listings/{id:guid}", async (
            Guid id,
            UpdateListingRequest request,
            HttpContext context,
            ISender sender) =>
        {
            var command = new UpdateListingCommand(
                id,
                request.Title,
                request.Description,
                request.Price,
                request.PricePerHour,
                request.PricePerNight,
                request.Rooms,
                request.AreaSquareMeters,
                request.District,
                request.Address,
                request.Latitude,
                request.Longitude,
                context.User.GetUserId());

            var result = await sender.Send(command);

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}

public sealed record UpdateListingRequest(
    string Title,
    string Description,
    decimal Price,
    decimal? PricePerHour,
    decimal? PricePerNight,
    int Rooms,
    double AreaSquareMeters,
    string District,
    string Address,
    double Latitude,
    double Longitude);
