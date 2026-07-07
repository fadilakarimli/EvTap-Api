using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.CreateListing;

public sealed record CreateListingCommand(
    string Title,
    string Description,
    decimal Price,
    int Rooms,
    double AreaSquareMeters,
    string District,
    string Address,
    double Latitude,
    double Longitude,
    Guid OwnerId) : IRequest<Result<Guid>>;
