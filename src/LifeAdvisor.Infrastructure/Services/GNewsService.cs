using System.Text.Json;
using System.Text.Json.Serialization;
using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using Microsoft.Extensions.Options;

namespace LifeAdvisor.Infrastructure.Services;

// Pulls real-world headlines from GNews (https://gnews.io). Free tier returns article
// images, which is what makes the briefing cards eye-catching. Swapping providers later
// means writing one more INewsService — nothing else changes.
public class GNewsService(IOptions<NewsSettings> options) : INewsService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(12) };
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly NewsSettings _settings = options.Value;

    public async Task<IReadOnlyList<NewsArticle>> SearchAsync(string query, int max, CancellationToken ct = default)
    {
        // No key configured → let the briefing fall back gracefully rather than break.
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) || string.IsNullOrWhiteSpace(query))
            return [];

        try
        {
            // Default sort (publishedAt) returns the latest matching stories; "sortby=relevance"
            // is too aggressive — for an exact-phrase query it collapses to a single article.
            var url = $"{_settings.BaseUrl.TrimEnd('/')}/search" +
                      $"?q={Uri.EscapeDataString(query)}" +
                      $"&lang=en&max={Math.Clamp(max, 1, 10)}" +
                      $"&apikey={Uri.EscapeDataString(_settings.ApiKey)}";

            using var response = await Http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
                return [];

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            var payload = await JsonSerializer.DeserializeAsync<GNewsResponse>(stream, JsonOptions, ct);

            if (payload?.Articles is null)
                return [];

            return payload.Articles
                .Where(a => !string.IsNullOrWhiteSpace(a.Title) && !string.IsNullOrWhiteSpace(a.Url))
                .Select(a => new NewsArticle(
                    a.Title!.Trim(),
                    (a.Description ?? string.Empty).Trim(),
                    a.Url!.Trim(),
                    (a.Image ?? string.Empty).Trim(),
                    a.Source?.Name?.Trim() ?? "News",
                    a.PublishedAt))
                .ToList();
        }
        catch
        {
            // Network blip, bad key, quota hit — the briefing degrades, the page never breaks.
            return [];
        }
    }

    private sealed class GNewsResponse
    {
        [JsonPropertyName("articles")]
        public List<GNewsArticle>? Articles { get; set; }
    }

    private sealed class GNewsArticle
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Image { get; set; }
        public DateTime? PublishedAt { get; set; }
        public GNewsSource? Source { get; set; }
    }

    private sealed class GNewsSource
    {
        public string? Name { get; set; }
    }
}
