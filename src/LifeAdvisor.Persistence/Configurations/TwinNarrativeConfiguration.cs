using System;
using System.Linq;
using System.Text.Json;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LifeAdvisor.Persistence.Configurations;

public class TwinNarrativeConfiguration : IEntityTypeConfiguration<TwinNarrative>
{
    public void Configure(EntityTypeBuilder<TwinNarrative> builder)
    {
        builder.HasKey(t => t.TwinNarrativeId);

        // Convert the Enum to a String in the database. 
        // This makes your database highly readable (e.g., you will see "DeepestFears" 
        // instead of the number "7" when querying the database directly).
        builder.Property(t => t.Type)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(t => t.Content)
               .IsRequired(); // Will default to NVARCHAR(MAX), which is perfect for AI context

        builder.Property(t => t.DecisionTitle)
               .HasMaxLength(300)
               .IsRequired();

        builder.Property(t => t.ScenarioOptionsJson)
               .IsRequired();

        builder.Property(t => t.SelectedScenarioTitle)
               .HasMaxLength(200)
               .IsRequired(false);

        // Store the embedding as a JSON array in an nvarchar(max) column.
        // Native SQL Server "vector" types require SQL Server 2025; this keeps the
        // schema compatible with SQL Server 2022 and earlier. Cosine similarity is
        // computed in C# (see RelatedDecisionRetriever), so no DB-side vector ops
        // are needed.
        var embeddingConverter = new ValueConverter<float[]?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<float[]>(v, (JsonSerializerOptions?)null));

        var embeddingComparer = new ValueComparer<float[]?>(
            (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
            v => v == null ? 0 : v.Aggregate(0, (acc, f) => HashCode.Combine(acc, f.GetHashCode())),
            v => v == null ? null : v.ToArray());

        builder.Property(t => t.Embedding)
               .HasConversion(embeddingConverter, embeddingComparer)
               .HasColumnType("nvarchar(max)")
               .IsRequired(false);

        builder.Property(t => t.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");
    }
}
