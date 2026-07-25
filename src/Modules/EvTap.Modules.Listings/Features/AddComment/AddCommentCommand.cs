using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.AddComment;

public sealed record AddCommentCommand(
    Guid ListingId,
    Guid AuthorId,
    string AuthorName,
    string Text) : IRequest<Result<CommentResponse>>;

public sealed record CommentResponse(
    Guid Id,
    Guid ListingId,
    Guid AuthorId,
    string AuthorName,
    string Text,
    DateTimeOffset CreatedAt);
