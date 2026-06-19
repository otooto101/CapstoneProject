using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

public interface IAnalysisSettingsService
{
    Task<AnalysisSettings> GetSettingsAsync(string userId, CancellationToken ct = default);
    Task UpdateSettingsAsync(string userId, AnalysisSettings settings, CancellationToken ct = default);
}
