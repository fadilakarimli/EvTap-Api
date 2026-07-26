using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.UpdateListing;

public sealed record UpdateListingCommand(
    Guid Id,
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
    double Longitude,
    Guid RequestingUserId) : IRequest<Result>;
