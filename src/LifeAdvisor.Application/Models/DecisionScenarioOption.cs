namespace LifeAdvisor.Application.Models;

public class DecisionScenarioOption
{
    public string Title { get; init; } = string.Empty;
    public int Score { get; init; }
    public string Summary { get; init; } = string.Empty;
    public string BestWhen { get; init; } = string.Empty;
    public string MainRisk { get; init; } = string.Empty;
    public string FirstStep { get; init; } = string.Empty;
}