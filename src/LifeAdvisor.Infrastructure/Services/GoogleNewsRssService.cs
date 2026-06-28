using System.Globalization;
using System.Xml.Linq;
using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Infrastructure.Services;

// Keyless real-world news via Google News RSS. No API key, no signup — so the briefing works
// out of the box. Trade-off vs GNews: these feeds rarely carry images, so cards fall back to
// the themed gradient until a photo-capable key (GNews) is configured.
public class GoogleNewsRssService : INewsService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(12) };

    public async Task<IReadOnlyList<NewsArticle>> SearchAsync(string query, int max, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        try
        {
            var url = $"https://news.google.com/rss/search?q={Uri.EscapeDataString(query)}&hl=en-US&gl=US&ceid=US:en";
            await using var stream = await Http.GetStreamAsync(url, ct);
            var doc = await XDocument.LoadAsync(stream, LoadOptions.None, ct);

            var results = new List<NewsArticle>();

            foreach (var item in doc.Descendants("item").Take(Math.Clamp(max, 1, 12)))
            {
                var rawTitle = (string?)item.Element("title") ?? string.Empty;
                var link = (string?)item.Element("link") ?? string.Empty;
                if (string.IsNullOrWhiteSpace(rawTitle) || string.IsNullOrWhiteSpace(link))
                    continue;

                var source = item.Element("source")?.Value?.Trim() ?? string.Empty;

                // Google News titles end with " - {Source}"; strip it for a clean headline,
                // and use it as the source name when the <source> element is missing.
                var title = rawTitle;
                var dashIdx = rawTitle.LastIndexOf(" - ", StringComparison.Ordinal);
                if (dashIdx > 0)
                {
                    title = rawTitle[..dashIdx].Trim();
                    if (string.IsNullOrWhiteSpace(source))
                        source = rawTitle[(dashIdx + 3)..].Trim();
                }

                DateTime? published = DateTime.TryParse(
                    (string?)item.Element("pubDate"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt)
                    ? dt
                    : null;

                results.Add(new NewsArticle(
                    title,
                    string.Empty,
                    link,
                    string.Empty,
                    string.IsNullOrWhiteSpace(source) ? "Google News" : source,
                    published));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }
}
