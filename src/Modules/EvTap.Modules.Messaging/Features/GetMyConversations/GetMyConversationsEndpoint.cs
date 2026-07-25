using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Messaging.Features.GetMyConversations;

public sealed class GetMyConversationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/messages", async (HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new GetMyConversationsQuery(context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
