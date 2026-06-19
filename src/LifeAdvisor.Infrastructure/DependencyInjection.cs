using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LifeAdvisor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<SemanticKernelSettings>(configuration.GetSection("SemanticKernel"));

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAnalysisSettingsService, AnalysisSettingsService>();
        services.AddSingleton<SemanticKernelFactory>();
        services.AddScoped<ITextEmbeddingService, SemanticKernelTextEmbeddingService>();
        services.AddScoped<IRelatedDecisionRetriever, RelatedDecisionRetriever>();
        services.AddScoped<IDecisionAnalysisService, SemanticKernelDecisionAnalysisService>();

        return services;
    }
}
