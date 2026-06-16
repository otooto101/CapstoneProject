// /src/LifeAdvisor.Domain/Entities/DigitalTwin.cs
using System;
using System.Collections.Generic;
using LifeAdvisor.Domain.ValueObjects; // Add this

namespace LifeAdvisor.Domain.Entities;

public class DigitalTwin
{
    public int DigitalTwinId { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;

    public string PreferredName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Address Location { get; set; } = null!;

    public int LifeStageOptionId { get; set; }
    public LifeStageOption LifeStage { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TwinLifeDomain> SelectedDomains { get; set; } = new List<TwinLifeDomain>();
    public ICollection<TwinNarrative> Narratives { get; set; } = new List<TwinNarrative>();
}