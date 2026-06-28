using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using Microsoft.Extensions.Options;

namespace LifeAdvisor.Infrastructure.Services;

// The news source the rest of the app talks to. Uses GNews (real photos) when an API key is
// configured; otherwise — or if GNews returns nothing (bad key / quota) — falls back to
// keyless Google News RSS so the briefing always has real stories.
public class PrimaryNewsService(
    GNewsService gnews,
    GoogleNewsRssService rss,
    IOptions<NewsSettings> options) : INewsService
{
    private readonly bool _hasKey = !string.IsNullOrWhiteSpace(options.Value.ApiKey);

    public async Task<IReadOnlyList<NewsArticle>> SearchAsync(string query, int max, CancellationToken ct = default)
    {
        if (_hasKey)
        {
            var fromGnews = await gnews.SearchAsync(query, max, ct);
            if (fromGnews.Count > 0)
                return fromGnews;
        }

        return await rss.SearchAsync(query, max, ct);
    }
}
