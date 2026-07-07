using EvTap.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EvTap.Modules.Listings.Features.UploadListingImages;

public sealed record UploadListingImagesCommand(
    Guid ListingId,
    Guid RequestingUserId,
    IFormFileCollection Files) : IRequest<Result<IReadOnlyList<string>>>;
