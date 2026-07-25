using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Messaging.Features.GetMyConversations;

/// <summary>The current user's conversations, one row per (listing, other participant),
/// with the latest message as a preview.</summary>
public sealed record GetMyConversationsQuery(Guid CurrentUserId)
    : IRequest<Result<IReadOnlyList<ConversationSummary>>>;

public sealed record ConversationSummary(
    Guid ListingId,
    string ListingTitle,
    Guid OtherUserId,
    string OtherUserName,
    string LastMessageText,
    DateTimeOffset LastMessageAt);
