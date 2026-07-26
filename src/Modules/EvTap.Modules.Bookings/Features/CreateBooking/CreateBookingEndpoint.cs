using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Bookings.Features.CreateBooking;

public sealed class CreateBookingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings", async (CreateBookingRequest request, HttpContext context, ISender sender) =>
        {
            var command = new CreateBookingCommand(
                request.ListingId,
                context.User.GetUserId(),
                context.User.GetUserEmail(),
                request.RentalType,
                request.StartAt,
                request.Units);

            var result = await sender.Send(command);

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}

public sealed record CreateBookingRequest(Guid ListingId, string RentalType, DateTimeOffset StartAt, int Units);
