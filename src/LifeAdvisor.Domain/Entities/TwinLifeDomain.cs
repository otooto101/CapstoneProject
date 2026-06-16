using System;

namespace LifeAdvisor.Domain.Entities;

public class TwinLifeDomain
{
    public int TwinLifeDomainId { get; set; }

    public int DigitalTwinId { get; set; }
    public DigitalTwin Twin { get; set; } = null!;

    public int LifeDomainOptionId { get; set; }
    public LifeDomainOption DomainOption { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}