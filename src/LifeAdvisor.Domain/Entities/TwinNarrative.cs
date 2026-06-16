using System;
using LifeAdvisor.Domain.Enums;
using Microsoft.Data.SqlTypes;

namespace LifeAdvisor.Domain.Entities;

public class TwinNarrative
{
    public int TwinNarrativeId { get; set; }

    public int DigitalTwinId { get; set; }
    public DigitalTwin Twin { get; set; } = null!;
    public NarrativeType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public SqlVector<float>? Embedding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}