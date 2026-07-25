using EvTap.Modules.Listings.Features.AddComment;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetComments;

public sealed record GetCommentsQuery(Guid ListingId) : IRequest<Result<IReadOnlyList<CommentResponse>>>;
