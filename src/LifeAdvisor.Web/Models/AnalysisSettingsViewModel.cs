using System.ComponentModel.DataAnnotations;

namespace LifeAdvisor.Web.Models;

public class AnalysisSettingsViewModel : DashboardShellViewModel
{
    [Range(1, 20)]
    [Display(Name = "Maximum related decisions")]
    public int MaxRelatedDecisions { get; set; } = 5;

    [Range(0, 1)]
    [Display(Name = "Similarity threshold")]
    public double SimilarityThreshold { get; set; } = 0.75;
}
