namespace LifeAdvisor.Application.Models;

public class DecisionAnalysisResult
{
    public int DecisionHistoryId { get; init; }
    public string Prompt { get; init; } = string.Empty;
    public string HistoryAcknowledgement { get; init; } = string.Empty;
    public string Guidance { get; init; } = string.Empty;
    public IReadOnlyList<DecisionScenarioOption> ScenarioOptions { get; init; } = [];
    public IReadOnlyList<RelatedDecisionMatch> RelatedDecisions { get; init; } = [];
    public int ConsideredDecisionCount { get; init; }
    public double SimilarityThreshold { get; init; }
}
