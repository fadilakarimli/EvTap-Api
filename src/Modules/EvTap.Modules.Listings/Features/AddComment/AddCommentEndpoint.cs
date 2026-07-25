using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Listings.Features.AddComment;

public sealed class AddCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/listings/{id:guid}/comments", async (
            Guid id,
            AddCommentRequest request,
            HttpContext context,
            ISender sender) =>
        {
            var command = new AddCommentCommand(
                id,
                context.User.GetUserId(),
                context.User.GetUserName(),
                request.Text);

            var result = await sender.Send(command);

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}

public sealed record AddCommentRequest(string Text);
