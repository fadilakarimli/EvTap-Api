using EvTap.Modules.Messaging.Features.SendMessage;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Messaging.Features.GetConversation;

/// <summary>All messages between the current user and another user about a specific listing.</summary>
public sealed record GetConversationQuery(
    Guid CurrentUserId,
    Guid OtherUserId,
    Guid ListingId) : IRequest<Result<IReadOnlyList<MessageResponse>>>;
