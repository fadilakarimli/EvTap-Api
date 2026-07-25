using EvTap.Modules.Listings.Features.AddComment;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetComments;

internal sealed class GetCommentsHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetCommentsQuery, Result<IReadOnlyList<CommentResponse>>>
{
    public async Task<Result<IReadOnlyList<CommentResponse>>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var comments = await dbContext.ListingComments
            .Where(c => c.ListingId == request.ListingId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse(
                c.Id,
                c.ListingId,
                c.AuthorId,
                c.AuthorName,
                c.Text,
                c.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CommentResponse>>(comments);
    }
}
