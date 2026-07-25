using EvTap.Modules.Users.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Users.Features.GetAllUsers;

internal sealed class GetAllUsersHandler(UsersDbContext dbContext)
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<AdminUserResponse>>>
{
    public async Task<Result<IReadOnlyList<AdminUserResponse>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserResponse(
                u.Id,
                u.Name,
                u.Email,
                u.Role.ToString(),
                u.IsEmailVerified,
                u.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AdminUserResponse>>(users);
    }
}
