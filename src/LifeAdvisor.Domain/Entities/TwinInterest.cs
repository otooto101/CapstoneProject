using System;

namespace LifeAdvisor.Domain.Entities;

// One user's response to a topic: swiped right (interested, with a priority) or left (passed).
public class TwinInterest
{
    public int TwinInterestId { get; set; }

    public int DigitalTwinId { get; set; }
    public DigitalTwin Twin { get; set; } = null!;

    public int InterestTopicId { get; set; }
    public InterestTopic Topic { get; set; } = null!;

    // true = swiped right (interested), false = swiped left (passed)
    public bool IsInterested { get; set; }

    // 0 when passed; 1 = curious, 2 = keen, 3 = obsessed
    public int Priority { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
