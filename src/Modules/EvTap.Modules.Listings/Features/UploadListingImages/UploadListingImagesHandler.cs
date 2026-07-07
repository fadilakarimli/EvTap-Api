using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.UploadListingImages;

internal sealed class UploadListingImagesHandler(ListingsDbContext dbContext, IWebHostEnvironment environment)
    : IRequestHandler<UploadListingImagesCommand, Result<IReadOnlyList<string>>>
{
    public async Task<Result<IReadOnlyList<string>>> Handle(
        UploadListingImagesCommand request,
        CancellationToken cancellationToken)
    {
        var listing = await dbContext.Listings
            .Include(l => l.Images)
            .SingleOrDefaultAsync(l => l.Id == request.ListingId, cancellationToken);

        if (listing is null)
        {
            return Result.Failure<IReadOnlyList<string>>(
                Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        if (listing.OwnerId != request.RequestingUserId)
        {
            return Result.Failure<IReadOnlyList<string>>(
                Error.Forbidden("Listings.NotOwner", "You do not own this listing."));
        }

        var uploadsDirectory = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDirectory);

        var nextOrder = listing.Images.Count == 0 ? 0 : listing.Images.Max(i => i.Order) + 1;
        var urls = new List<string>();

        foreach (var file in request.Files)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var physicalPath = Path.Combine(uploadsDirectory, fileName);

            await using (var stream = File.Create(physicalPath))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var url = $"/uploads/{fileName}";

            dbContext.ListingImages.Add(new ListingImage
            {
                Id = Guid.NewGuid(),
                ListingId = listing.Id,
                Url = url,
                Order = nextOrder++,
            });

            urls.Add(url);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<string>>(urls);
    }
}
