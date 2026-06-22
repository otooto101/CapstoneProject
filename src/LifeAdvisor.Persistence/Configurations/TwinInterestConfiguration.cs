using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class TwinInterestConfiguration : IEntityTypeConfiguration<TwinInterest>
{
    public void Configure(EntityTypeBuilder<TwinInterest> builder)
    {
        builder.HasKey(i => i.TwinInterestId);

        builder.HasOne(i => i.Twin)
               .WithMany()
               .HasForeignKey(i => i.DigitalTwinId)
               .OnDelete(DeleteBehavior.Cascade);

        // Restrict on the topic side so we don't create two cascade paths into
        // TwinInterests (SQL Server rejects that). Topics are seed data anyway.
        builder.HasOne(i => i.Topic)
               .WithMany(t => t.Interests)
               .HasForeignKey(i => i.InterestTopicId)
               .OnDelete(DeleteBehavior.Restrict);

        // One response per (twin, topic) so swiping again updates the same row.
        builder.HasIndex(i => new { i.DigitalTwinId, i.InterestTopicId }).IsUnique();

        builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
