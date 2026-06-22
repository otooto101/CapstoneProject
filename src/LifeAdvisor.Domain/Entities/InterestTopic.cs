using System.Collections.Generic;

namespace LifeAdvisor.Domain.Entities;

// A curated topic the user can swipe on to teach their twin what they care about.
public class InterestTopic
{
    public int InterestTopicId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public ICollection<TwinInterest> Interests { get; set; } = new List<TwinInterest>();
}
