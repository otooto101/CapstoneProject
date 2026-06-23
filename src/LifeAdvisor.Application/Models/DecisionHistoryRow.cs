namespace LifeAdvisor.Application.Models;

// Lightweight projection of a decision narrative used for the history list.
// Deliberately excludes the embedding vector so the history query stays cheap.
public class DecisionHistoryRow
{
    public int TwinNarrativeId { get; init; }
    public string DecisionTitle { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public bool IsCompletedDecision { get; init; }
    public string? SelectedScenarioTitle { get; init; }
    public string ScenarioOptionsJson { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
