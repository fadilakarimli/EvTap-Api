using System.Security.Claims;

namespace EvTap.Shared.Security;

public static class CurrentUserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User id claim not found on the current principal.");

        return Guid.Parse(id);
    }
}
