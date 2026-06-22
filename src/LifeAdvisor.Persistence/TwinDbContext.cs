using LifeAdvisor.Domain.Entities;
using LifeAdvisor.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LifeAdvisor.Persistence;

public class TwinDbContext : IdentityDbContext<ApplicationUser>
{
    public TwinDbContext(DbContextOptions<TwinDbContext> options) : base(options)
    {
    }

    public DbSet<DigitalTwin> DigitalTwins => Set<DigitalTwin>();
    public DbSet<TwinNarrative> TwinNarratives => Set<TwinNarrative>();
    public DbSet<TwinLifeDomain> TwinLifeDomains => Set<TwinLifeDomain>();
    public DbSet<LifeStageOption> LifeStageOptions => Set<LifeStageOption>();
    public DbSet<LifeDomainOption> LifeDomainOptions => Set<LifeDomainOption>();
    public DbSet<InterestTopic> InterestTopics => Set<InterestTopic>();
    public DbSet<TwinInterest> TwinInterests => Set<TwinInterest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TwinDbContext).Assembly);
    }
}