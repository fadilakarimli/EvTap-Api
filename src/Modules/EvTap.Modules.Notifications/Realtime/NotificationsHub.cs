using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EvTap.Modules.Notifications.Realtime;

/// <summary>
/// Real-time notification hub. Clients connect with their JWT; SignalR's default
/// IUserIdProvider keys connections by the NameIdentifier claim, so the consumer can push
/// to a specific user with Clients.User(userId).
/// </summary>
[Authorize]
public sealed class NotificationsHub : Hub;
