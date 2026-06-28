using System;
using System.Collections.Generic;

namespace LifeAdvisor.Domain.Entities;

// One day's personalized briefing for a twin: an AI-written greeting plus a set of
// real-world news items chosen from the topics the user swiped on. Persisted so the
// dashboard is instant on entry and the news quota isn't burned on every page load.
public class DailyBriefing
{
    public int DailyBriefingId { get; set; }

    public int DigitalTwinId { get; set; }
    public DigitalTwin Twin { get; set; } = null!;

    // The local date this briefing covers (one per twin per day).
    public DateOnly BriefingDate { get; set; }

    // The twin's opening words — addresses the user by name and reflects their interests.
    public string Greeting { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BriefingItem> Items { get; set; } = new List<BriefingItem>();
}
