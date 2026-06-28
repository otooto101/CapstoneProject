using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;

public record DailyBriefingDto(
    string Greeting,
    DateTime GeneratedAt,
    IReadOnlyList<BriefingItemDto> Items)
{
    public static DailyBriefingDto From(DailyBriefing briefing) => new(
        briefing.Greeting,
        briefing.GeneratedAt,
        briefing.Items
            .OrderBy(item => item.DisplayOrder)
            .Select(BriefingItemDto.From)
            .ToList());
}

public record BriefingItemDto(
    string Headline,
    string Blurb,
    string WhyItMatters,
    string SourceName,
    string Url,
    string ImageUrl,
    string MatchedInterest,
    DateTime? PublishedAt)
{
    public static BriefingItemDto From(BriefingItem item) => new(
        item.Headline,
        item.Blurb,
        item.WhyItMatters,
        item.SourceName,
        item.Url,
        item.ImageUrl,
        item.MatchedInterest,
        item.PublishedAt);
}
