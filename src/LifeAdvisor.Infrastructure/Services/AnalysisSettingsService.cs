using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace LifeAdvisor.Infrastructure.Services;

public class AnalysisSettingsService(UserManager<ApplicationUser> userManager) : IAnalysisSettingsService
{
    public async Task<AnalysisSettings> GetSettingsAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        return new AnalysisSettings
        {
            MaxRelatedDecisions = user.MaxRelatedDecisions <= 0 ? AnalysisSettings.DefaultMaxRelatedDecisions : user.MaxRelatedDecisions,
            SimilarityThreshold = user.SimilarityThreshold is < 0 or > 1
                ? AnalysisSettings.DefaultSimilarityThreshold
                : user.SimilarityThreshold
        };
    }

    public async Task UpdateSettingsAsync(string userId, AnalysisSettings settings, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        user.MaxRelatedDecisions = settings.MaxRelatedDecisions;
        user.SimilarityThreshold = settings.SimilarityThreshold;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
    }
}
