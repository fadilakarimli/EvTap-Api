using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Users.Features.ResendOtp;

public sealed class ResendOtpEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/resend-otp", async (ResendOtpRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ResendOtpCommand(request.Email));

            return result.ToHttpResult();
        });
    }
}

public sealed record ResendOtpRequest(string Email);
