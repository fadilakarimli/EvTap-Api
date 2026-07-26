using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.CreateListing;

public sealed class CreateListingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/listings", async (CreateListingRequest request, HttpContext context, ISender sender) =>
        {
            var ownerId = context.User.GetUserId();

            var command = new CreateListingCommand(
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
                ownerId);

            var result = await sender.Send(command);

            return result.ToCreatedResult(id => $"/api/listings/{id}");
        }).RequireAuthorization();
    }
}

public sealed record CreateListingRequest(
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
