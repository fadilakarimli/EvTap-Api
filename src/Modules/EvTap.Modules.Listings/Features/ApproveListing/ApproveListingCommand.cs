using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.ApproveListing;

public sealed record ApproveListingCommand(Guid Id) : IRequest<Result>;
