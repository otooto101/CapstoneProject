using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class LifeDomainOptionConfiguration : IEntityTypeConfiguration<LifeDomainOption>
{
    public void Configure(EntityTypeBuilder<LifeDomainOption> builder)
    {
        builder.HasKey(l => l.LifeDomainOptionId);

        builder.Property(l => l.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(l => l.IsActive)
               .HasDefaultValue(true);

        builder.Property(l => l.DisplayOrder)
               .IsRequired();
    }
}