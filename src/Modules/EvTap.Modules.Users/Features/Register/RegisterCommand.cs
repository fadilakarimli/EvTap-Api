using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Users.Features.Register;

public sealed record RegisterCommand(string Name, string Email, string Password) : IRequest<Result<Guid>>;
