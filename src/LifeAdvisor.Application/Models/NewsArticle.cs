namespace LifeAdvisor.Application.Models;

// A single real-world article returned by the news source, normalized so the rest of
// the app never depends on a specific provider's response shape.
public record NewsArticle(
    string Title,
    string Description,
    string Url,
    string ImageUrl,
    string SourceName,
    DateTime? PublishedAt);
