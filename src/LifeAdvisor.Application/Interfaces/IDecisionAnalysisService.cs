using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

public interface IDecisionAnalysisService
{
    Task<DecisionAnalysisResult> AnalyzeAsync(string identityUserId, string prompt, CancellationToken ct = default);
    Task CompleteDecisionAsync(string identityUserId, int decisionHistoryId, string selectedScenarioTitle, CancellationToken ct = default);
    Task<IReadOnlyList<DecisionHistoryItem>> GetDecisionHistoryAsync(string identityUserId, CancellationToken ct = default);
}
