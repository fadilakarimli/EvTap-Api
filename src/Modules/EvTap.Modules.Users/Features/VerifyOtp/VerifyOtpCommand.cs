using EvTap.Modules.Users.Features.Login;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Users.Features.VerifyOtp;

/// <summary>Confirms the 6-digit code sent to the email; on success the account becomes
/// verified and a JWT is returned so the client can continue straight into the app.</summary>
public sealed record VerifyOtpCommand(string Email, string Code) : IRequest<Result<LoginResponse>>;
