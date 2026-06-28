using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class DailyBriefingConfiguration : IEntityTypeConfiguration<DailyBriefing>
{
    public void Configure(EntityTypeBuilder<DailyBriefing> builder)
    {
        builder.HasKey(b => b.DailyBriefingId);

        builder.Property(b => b.Greeting).IsRequired().HasMaxLength(2000);
        builder.Property(b => b.BriefingDate).IsRequired();
        builder.Property(b => b.GeneratedAt).IsRequired();

        builder.HasOne(b => b.Twin)
            .WithMany()
            .HasForeignKey(b => b.DigitalTwinId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Items)
            .WithOne(i => i.Briefing)
            .HasForeignKey(i => i.DailyBriefingId)
            .OnDelete(DeleteBehavior.Cascade);

        // One briefing per twin per day.
        builder.HasIndex(b => new { b.DigitalTwinId, b.BriefingDate }).IsUnique();
    }
}
