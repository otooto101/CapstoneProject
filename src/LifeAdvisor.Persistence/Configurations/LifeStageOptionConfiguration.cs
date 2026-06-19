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

        builder.HasData(
            new LifeStageOption
            {
                LifeStageOptionId = 1,
                Name = "Student / Early Career",
                IsActive = true,
                DisplayOrder = 1
            },
            new LifeStageOption
            {
                LifeStageOptionId = 2,
                Name = "Building Career",
                IsActive = true,
                DisplayOrder = 2
            },
            new LifeStageOption
            {
                LifeStageOptionId = 3,
                Name = "Settling Down / Starting Family",
                IsActive = true,
                DisplayOrder = 3
            },
            new LifeStageOption
            {
                LifeStageOptionId = 4,
                Name = "Parenting / Raising Children",
                IsActive = true,
                DisplayOrder = 4
            },
            new LifeStageOption
            {
                LifeStageOptionId = 5,
                Name = "Career Transition / Reinvention",
                IsActive = true,
                DisplayOrder = 5
            },
            new LifeStageOption
            {
                LifeStageOptionId = 6,
                Name = "Midlife Growth / Reflection",
                IsActive = true,
                DisplayOrder = 6
            },
            new LifeStageOption
            {
                LifeStageOptionId = 7,
                Name = "Pre-Retirement Planning",
                IsActive = true,
                DisplayOrder = 7
            },
            new LifeStageOption
            {
                LifeStageOptionId = 8,
                Name = "Retirement / Later Life",
                IsActive = true,
                DisplayOrder = 8
            });
    }
}