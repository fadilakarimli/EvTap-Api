using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Users.Features.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc);
