using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

// Pulls real-world headlines from an external source. Implementations must never throw
// for a missing key or a provider error — they return an empty list so the briefing can
// fall back gracefully.
public interface INewsService
{
    Task<IReadOnlyList<NewsArticle>> SearchAsync(string query, int max, CancellationToken ct = default);
}
