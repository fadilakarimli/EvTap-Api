using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Messaging.Features.GetConversation;

public sealed class GetConversationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/messages/{listingId:guid}/{otherUserId:guid}", async (
            Guid listingId,
            Guid otherUserId,
            HttpContext context,
            ISender sender) =>
        {
            var result = await sender.Send(
                new GetConversationQuery(context.User.GetUserId(), otherUserId, listingId));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
