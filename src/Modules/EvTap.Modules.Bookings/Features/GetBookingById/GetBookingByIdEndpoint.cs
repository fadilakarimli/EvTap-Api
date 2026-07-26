using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Bookings.Features.GetBookingById;

public sealed class GetBookingByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{id:guid}", async (Guid id, HttpContext context, ISender sender) =>
        {
            var result = await sender.Send(new GetBookingByIdQuery(id, context.User.GetUserId()));

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
