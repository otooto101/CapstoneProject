namespace LifeAdvisor.Application.Models;

public class AnalysisSettings
{
    public const int DefaultMaxRelatedDecisions = 5;
    public const double DefaultSimilarityThreshold = 0.75;

    public int MaxRelatedDecisions { get; set; } = DefaultMaxRelatedDecisions;
    public double SimilarityThreshold { get; set; } = DefaultSimilarityThreshold;
}
