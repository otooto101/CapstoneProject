using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class DigitalTwinConfiguration : IEntityTypeConfiguration<DigitalTwin>
{
    public void Configure(EntityTypeBuilder<DigitalTwin> builder)
    {
        builder.HasKey(t => t.DigitalTwinId);

        builder.Property(t => t.PreferredName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(t => t.IdentityUserId)
               .IsRequired()
               .HasMaxLength(450);

        builder.Property(t => t.DateOfBirth)
               .IsRequired();

        builder.ComplexProperty(t => t.Location, addressBuilder =>
        {
            addressBuilder.Property(a => a.City)
                          .HasMaxLength(100)
                          .IsRequired()
                          .HasColumnName("City");

            addressBuilder.Property(a => a.Country)
                          .HasMaxLength(100)
                          .IsRequired()
                          .HasColumnName("Country");
        });

        builder.Property(t => t.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // Relationships

        // DigitalTwin -> LifeStageOption (One-to-Many)
        builder.HasOne(t => t.LifeStage)
               .WithMany(s => s.Twins)
               .HasForeignKey(t => t.LifeStageOptionId)
               .OnDelete(DeleteBehavior.Restrict);

        // DigitalTwin -> TwinLifeDomain (One-to-Many - The Join Table)
        builder.HasMany(t => t.SelectedDomains)
               .WithOne(d => d.Twin)
               .HasForeignKey(d => d.DigitalTwinId)
               .OnDelete(DeleteBehavior.Cascade);

        // DigitalTwin -> TwinNarrative (One-to-Many - The Vector Table)
        builder.HasMany(t => t.Narratives)
               .WithOne(n => n.Twin)
               .HasForeignKey(n => n.DigitalTwinId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}