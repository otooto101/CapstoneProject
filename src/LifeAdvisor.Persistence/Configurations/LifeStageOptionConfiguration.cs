using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class LifeStageOptionConfiguration : IEntityTypeConfiguration<LifeStageOption>
{
    public void Configure(EntityTypeBuilder<LifeStageOption> builder)
    {
        builder.HasKey(l => l.LifeStageOptionId);

        builder.Property(l => l.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(l => l.IsActive)
               .HasDefaultValue(true);

        builder.Property(l => l.DisplayOrder)
               .IsRequired();
    }
}