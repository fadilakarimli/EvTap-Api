using EvTap.Modules.SavedSearches.Domain;
using EvTap.Modules.SavedSearches.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.SavedSearches.Features.CreateSavedSearch;

internal sealed class CreateSavedSearchHandler(SavedSearchesDbContext dbContext)
    : IRequestHandler<CreateSavedSearchCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSavedSearchCommand request, CancellationToken cancellationToken)
    {
        var savedSearch = new SavedSearch
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            MinRooms = request.MinRooms,
            District = request.District,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.SavedSearches.Add(savedSearch);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(savedSearch.Id);
    }
}
