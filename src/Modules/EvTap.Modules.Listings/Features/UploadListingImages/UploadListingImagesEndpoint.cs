using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.UploadListingImages;

public sealed class UploadListingImagesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/listings/{id:guid}/images", async (
            Guid id,
            [FromForm] IFormFileCollection files,
            HttpContext context,
            ISender sender) =>
        {
            var command = new UploadListingImagesCommand(id, context.User.GetUserId(), files);
            var result = await sender.Send(command);

            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}
