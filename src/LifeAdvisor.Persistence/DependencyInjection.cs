using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Persistence.Identity;
using LifeAdvisor.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LifeAdvisor.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<TwinDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<TwinDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDigitalTwinRepository, DigitalTwinRepository>();
        services.AddScoped<ILifeStageOptionRepository, LifeStageOptionRepository>();
        services.AddScoped<ITwinNarrativeRepository, TwinNarrativeRepository>();
        services.AddScoped<IInterestTopicRepository, InterestTopicRepository>();
        services.AddScoped<ITwinInterestRepository, TwinInterestRepository>();
        services.AddScoped<IDailyBriefingRepository, DailyBriefingRepository>();

        return services;
    }
}
