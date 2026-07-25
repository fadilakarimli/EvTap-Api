using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Users.Features.GetAllUsers;

/// <summary>Admin-only: every registered user.</summary>
public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyList<AdminUserResponse>>>;

public sealed record AdminUserResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsEmailVerified,
    DateTimeOffset CreatedAt);
