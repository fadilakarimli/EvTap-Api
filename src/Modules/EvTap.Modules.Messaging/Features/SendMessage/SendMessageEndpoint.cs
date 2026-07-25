using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Messaging.Features.SendMessage;

public sealed class SendMessageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/messages", async (SendMessageRequest request, HttpContext context, ISender sender) =>
        {
            var command = new SendMessageCommand(
                request.ListingId,
                request.ListingTitle,
                context.User.GetUserId(),
                context.User.GetUserName(),
                request.RecipientId,
                request.Text);

            var result = await sender.Send(command);

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}

public sealed record SendMessageRequest(
    Guid ListingId,
    string ListingTitle,
    Guid RecipientId,
    string Text);
