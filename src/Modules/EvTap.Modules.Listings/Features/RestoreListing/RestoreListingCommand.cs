using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.RestoreListing;

public sealed record RestoreListingCommand(Guid Id) : IRequest<Result>;
