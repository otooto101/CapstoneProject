using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Interfaces;

// Orchestrates a personalized daily briefing: reads the user's swipe interests, pulls
// matching real-world news, has the twin synthesize a greeting + per-item takes, and
// persists the result (one per twin per day).
public interface IDailyBriefingService
{
    // Returns today's briefing, generating and saving it if one doesn't exist yet.
    Task<DailyBriefing> GetOrCreateTodayAsync(string identityUserId, CancellationToken ct = default);

    // Discards today's briefing (if any) and regenerates a fresh one.
    Task<DailyBriefing> RegenerateTodayAsync(string identityUserId, CancellationToken ct = default);
}
