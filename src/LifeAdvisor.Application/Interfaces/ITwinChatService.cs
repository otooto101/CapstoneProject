using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

// Powers the interactive "talk it through with your twin" surface. The twin opens the
// conversation itself, then answers grounded in the user's profile, interests, and today's
// briefing.
public interface ITwinChatService
{
    // The twin's first message — a warm, specific question that starts the conversation.
    Task<string> GetOpeningMessageAsync(string identityUserId, CancellationToken ct = default);

    // A reply to the user's latest message given the running conversation history.
    Task<string> ReplyAsync(string identityUserId, IReadOnlyList<ChatTurn> history, string message, CancellationToken ct = default);
}
