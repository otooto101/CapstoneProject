using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class BriefingItemConfiguration : IEntityTypeConfiguration<BriefingItem>
{
    public void Configure(EntityTypeBuilder<BriefingItem> builder)
    {
        builder.HasKey(i => i.BriefingItemId);

        builder.Property(i => i.Headline).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Blurb).HasMaxLength(2000);
        builder.Property(i => i.WhyItMatters).HasMaxLength(600);
        builder.Property(i => i.SourceName).HasMaxLength(200);
        builder.Property(i => i.Url).HasMaxLength(1000);
        builder.Property(i => i.ImageUrl).HasMaxLength(1000);
        builder.Property(i => i.MatchedInterest).HasMaxLength(120);
        builder.Property(i => i.DisplayOrder).IsRequired();
    }
}
