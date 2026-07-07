using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.SavedSearches.Features.CreateSavedSearch;

public sealed class CreateSavedSearchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/saved-searches", async (
            CreateSavedSearchRequest request,
            HttpContext context,
            ISender sender) =>
        {
            var command = new CreateSavedSearchCommand(
                context.User.GetUserId(),
                request.MinPrice,
                request.MaxPrice,
                request.MinRooms,
                request.District);

            var result = await sender.Send(command);

            return result.ToCreatedResult(id => $"/api/saved-searches/{id}");
        }).RequireAuthorization();
    }
}

public sealed record CreateSavedSearchRequest(
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinRooms,
    string? District);
