using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.DeleteListing;

public sealed record DeleteListingCommand(Guid Id, Guid RequestingUserId, bool IsAdmin) : IRequest<Result>;
