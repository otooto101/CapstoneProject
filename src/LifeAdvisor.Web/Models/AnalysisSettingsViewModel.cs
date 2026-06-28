using System.ComponentModel.DataAnnotations;
using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Web.Models;

public class AnalysisSettingsViewModel : DashboardShellViewModel
{
    [Range(1, 20)]
    [Display(Name = "Maximum related decisions")]
    public int MaxRelatedDecisions { get; set; } = AnalysisSettings.DefaultMaxRelatedDecisions;

    [Range(0, 1)]
    [Display(Name = "Similarity threshold")]
    public double SimilarityThreshold { get; set; } = AnalysisSettings.DefaultSimilarityThreshold;
}
