using System.Text;
using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LifeAdvisor.Infrastructure.Services;

// The interactive "talk it through with your twin" surface. The twin opens the conversation
// itself and answers grounded in the user's profile, interests, and today's briefing.
public class TwinChatService(
    IDigitalTwinRepository digitalTwinRepository,
    ITwinInterestRepository twinInterestRepository,
    IDailyBriefingRepository dailyBriefingRepository,
    SemanticKernelFactory semanticKernelFactory) : ITwinChatService
{
    public async Task<string> GetOpeningMessageAsync(string identityUserId, CancellationToken ct = default)
    {
        var context = await BuildContextAsync(identityUserId, ct);

        try
        {
            var history = new ChatHistory(BuildSystemPrompt(context));
            history.AddUserMessage(
                "Open the conversation yourself. Address me by name and get to the point — make a clear, plain " +
                "observation tied to today's briefing or my interests, and give me your honest read or what you'd focus on. " +
                "You may end with one simple question, but don't interrogate or push. Keep it under 40 words, calm and direct. No bullet lists.");

            var reply = await CompleteAsync(history, ct);
            return string.IsNullOrWhiteSpace(reply) ? FallbackOpening(context.Name) : reply;
        }
        catch
        {
            return FallbackOpening(context.Name);
        }
    }

    public async Task<string> ReplyAsync(string identityUserId, IReadOnlyList<ChatTurn> history, string message, CancellationToken ct = default)
    {
        var context = await BuildContextAsync(identityUserId, ct);

        try
        {
            var chat = new ChatHistory(BuildSystemPrompt(context));

            foreach (var turn in history.TakeLast(12))
            {
                if (string.IsNullOrWhiteSpace(turn.Content))
                    continue;

                if (string.Equals(turn.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                    chat.AddAssistantMessage(turn.Content);
                else
                    chat.AddUserMessage(turn.Content);
            }

            chat.AddUserMessage(message);

            var reply = await CompleteAsync(chat, ct);
            return string.IsNullOrWhiteSpace(reply)
                ? "I'm here — could you say a little more about that?"
                : reply;
        }
        catch
        {
            return "I'm having trouble gathering my thoughts right now. Give me a moment and try again.";
        }
    }

    private async Task<string> CompleteAsync(ChatHistory history, CancellationToken ct)
    {
        var kernel = semanticKernelFactory.CreateKernel();
        var chat = kernel.Services.GetRequiredService<IChatCompletionService>();
        var response = await chat.GetChatMessageContentAsync(history, cancellationToken: ct);
        return response.Content?.Trim() ?? string.Empty;
    }

    private static string BuildSystemPrompt(TwinContext context)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"You are {context.Name}'s personal digital twin. You tell them the truth plainly and you make the call yourself — no hedging, no sugarcoating.");
        prompt.AppendLine("Speak in the second person, stay concise (2-5 sentences), and be calm and matter-of-fact — direct, never aggressive or preachy. State things as they are. If something isn't okay, say so plainly; if it is, say that too.");
        prompt.AppendLine("Form your own judgment: give a clear, honest assessment and a recommendation instead of bouncing the decision back with 'what do you think?'. When something needs doing, say what you'd do.");
        prompt.AppendLine("Keep them grounded in reality and aware of what's going on in the world, but deliver it evenly — no lectures, no theatrics, no softening the facts. You never invent facts about them; when unsure, you ask.");

        if (!string.IsNullOrWhiteSpace(context.LifeStage))
            prompt.AppendLine($"Their life stage: {context.LifeStage}.");
        if (context.Domains.Count > 0)
            prompt.AppendLine($"Focus areas they chose: {string.Join(", ", context.Domains)}.");
        if (context.Interests.Count > 0)
            prompt.AppendLine($"Topics they're into (most important first): {string.Join(", ", context.Interests)}.");

        if (context.Headlines.Count > 0)
        {
            prompt.AppendLine("Today's briefing you prepared for them included:");
            foreach (var headline in context.Headlines)
                prompt.AppendLine($"- {headline}");
        }

        return prompt.ToString();
    }

    private async Task<TwinContext> BuildContextAsync(string identityUserId, CancellationToken ct)
    {
        var twin = await digitalTwinRepository.GetWithProfileAsync(identityUserId, ct);
        var name = string.IsNullOrWhiteSpace(twin?.PreferredName) ? "there" : twin!.PreferredName.Trim();

        var interests = (await twinInterestRepository.ListInterestedByUserAsync(identityUserId, ct))
            .Select(i => i.Topic.Title)
            .ToList();

        var domains = twin?.SelectedDomains?
            .Select(d => d.DomainOption?.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!)
            .ToList() ?? [];

        var headlines = new List<string>();
        if (twin is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var briefing = await dailyBriefingRepository.GetForTwinOnDateAsync(twin.DigitalTwinId, today, ct);
            if (briefing is not null)
                headlines = briefing.Items.OrderBy(i => i.DisplayOrder).Select(i => i.Headline).Take(6).ToList();
        }

        return new TwinContext(name, twin?.LifeStage?.Name ?? string.Empty, domains, interests, headlines);
    }

    private static string FallbackOpening(string name)
        => $"Hi {name} — I'm your twin. What's on your mind today? It can be something from the briefing or anything you're weighing.";

    private sealed record TwinContext(
        string Name,
        string LifeStage,
        IReadOnlyList<string> Domains,
        IReadOnlyList<string> Interests,
        IReadOnlyList<string> Headlines);
}
