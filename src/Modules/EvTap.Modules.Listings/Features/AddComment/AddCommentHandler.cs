using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.AddComment;

internal sealed class AddCommentHandler(ListingsDbContext dbContext)
    : IRequestHandler<AddCommentCommand, Result<CommentResponse>>
{
    public async Task<Result<CommentResponse>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var listingExists = await dbContext.Listings
            .AnyAsync(l => l.Id == request.ListingId, cancellationToken);

        if (!listingExists)
        {
            return Result.Failure<CommentResponse>(Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        var comment = new ListingComment
        {
            Id = Guid.NewGuid(),
            ListingId = request.ListingId,
            AuthorId = request.AuthorId,
            AuthorName = request.AuthorName,
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.ListingComments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CommentResponse(
            comment.Id,
            comment.ListingId,
            comment.AuthorId,
            comment.AuthorName,
            comment.Text,
            comment.CreatedAt));
    }
}
