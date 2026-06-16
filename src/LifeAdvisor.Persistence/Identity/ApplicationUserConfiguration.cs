using LifeAdvisor.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Id)
               .HasColumnName("ApplicationUserId");

        builder.Property(u => u.RefreshToken)
               .HasMaxLength(500);

        builder.Property(u => u.RefreshTokenExpiryTime)
               .IsRequired();

        builder.Property(u => u.RefreshTokenCreated)
               .IsRequired();

        builder.HasIndex(u => u.RefreshToken)
               .HasDatabaseName("IX_ApplicationUser_RefreshToken");

        builder.ToTable("Users");
    }
}