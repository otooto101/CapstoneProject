using System.ComponentModel.DataAnnotations;
using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Web.Models;

public class AnalyzeDecisionViewModel : DashboardShellViewModel
{
    [Required]
    [Display(Name = "What's weighing on you?")]
    public string Prompt { get; set; } = string.Empty;

    public string Guidance { get; set; } = string.Empty;
    public string HistoryAcknowledgement { get; set; } = string.Empty;
    public int DecisionHistoryId { get; set; }
    public string SelectedScenarioTitle { get; set; } = string.Empty;
    public int ConsideredDecisionCount { get; set; }
    public double SimilarityThreshold { get; set; }
    public IReadOnlyList<DecisionScenarioOption> ScenarioOptions { get; set; } = [];
    public IReadOnlyList<RelatedDecisionMatch> RelatedDecisions { get; set; } = [];
}
