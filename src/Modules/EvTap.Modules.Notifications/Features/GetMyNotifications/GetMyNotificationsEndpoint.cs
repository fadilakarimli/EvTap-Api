using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Notifications.Features.GetMyNotifications;

public sealed class GetMyNotificationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications", async (HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new GetMyNotificationsQuery(context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
