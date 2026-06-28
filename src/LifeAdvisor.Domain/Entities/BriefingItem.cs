using System;

namespace LifeAdvisor.Domain.Entities;

// A single real-world headline inside a DailyBriefing, tied back to the interest that
// surfaced it and carrying the twin's one-line take on why it matters to this person.
public class BriefingItem
{
    public int BriefingItemId { get; set; }

    public int DailyBriefingId { get; set; }
    public DailyBriefing Briefing { get; set; } = null!;

    public string Headline { get; set; } = string.Empty;
    public string Blurb { get; set; } = string.Empty;

    // The twin's personalized "why this matters to you" line.
    public string WhyItMatters { get; set; } = string.Empty;

    public string SourceName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // The swipe interest (topic title/category) this item was matched to.
    public string MatchedInterest { get; set; } = string.Empty;

    public DateTime? PublishedAt { get; set; }
    public int DisplayOrder { get; set; }
}
