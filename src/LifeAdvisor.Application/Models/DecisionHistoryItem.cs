namespace LifeAdvisor.Application.Models;

public class DecisionHistoryItem
{
    public int TwinNarrativeId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Prompt { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public string StatusLabel => IsCompleted ? "Completed" : "Non-complete";
    public string? SelectedScenarioTitle { get; init; }
    public IReadOnlyList<DecisionScenarioOption> ScenarioOptions { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}