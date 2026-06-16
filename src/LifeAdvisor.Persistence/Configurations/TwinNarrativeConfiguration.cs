using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        // Store embedding as float array in the vector column
        builder.Property(t => t.Embedding)
               .HasColumnType("vector(1536)")
               .IsRequired(false);

        builder.Property(t => t.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");
    }
}
