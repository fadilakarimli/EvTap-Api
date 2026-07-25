using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Users.Features.ResendOtp;

public sealed record ResendOtpCommand(string Email) : IRequest<Result>;
