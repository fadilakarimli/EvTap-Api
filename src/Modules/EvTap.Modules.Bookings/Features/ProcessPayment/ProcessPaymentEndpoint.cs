using Carter;
using EvTap.Shared.Endpoints;
using EvTap.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Bookings.Features.ProcessPayment;

public sealed class ProcessPaymentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{id:guid}/pay", async (Guid id, ProcessPaymentRequest request, HttpContext context, ISender sender) =>
        {
            var command = new ProcessPaymentCommand(
                id,
                context.User.GetUserId(),
                request.CardNumber,
                request.CardholderName,
                request.ExpiryMonth,
                request.ExpiryYear,
                request.Cvv);

            var result = await sender.Send(command);

            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}

public sealed record ProcessPaymentRequest(
    string CardNumber,
    string CardholderName,
    string ExpiryMonth,
    string ExpiryYear,
    string Cvv);
