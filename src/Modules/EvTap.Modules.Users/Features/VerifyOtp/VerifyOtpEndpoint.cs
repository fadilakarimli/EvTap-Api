using Carter;
using EvTap.Shared.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace EvTap.Modules.Users.Features.VerifyOtp;

public sealed class VerifyOtpEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/verify-otp", async (VerifyOtpRequest request, ISender sender) =>
        {
            var result = await sender.Send(new VerifyOtpCommand(request.Email, request.Code));

            return result.ToHttpResult();
        });
    }
}

public sealed record VerifyOtpRequest(string Email, string Code);
