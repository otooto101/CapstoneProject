using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class TwinLifeDomainConfiguration : IEntityTypeConfiguration<TwinLifeDomain>
{
    public void Configure(EntityTypeBuilder<TwinLifeDomain> builder)
    {
        builder.HasKey(t => t.TwinLifeDomainId);

        builder.Property(t => t.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // Relationships

        // TwinLifeDomain -> LifeDomainOption
        builder.HasOne(t => t.DomainOption)
               .WithMany(d => d.SelectedByTwins)
               .HasForeignKey(t => t.LifeDomainOptionId)
               // Restrict: Don't let an admin delete "Health & Wellness" if users have selected it
               .OnDelete(DeleteBehavior.Restrict);

        // Note: The relationship back to DigitalTwin is already configured 
        // in DigitalTwinConfiguration, so we don't need to repeat it here!
    }
}