namespace LifeAdvisor.Application.Models;

public class AnalysisSettings
{
    // Logical defaults applied to every user. Users can change these later in Settings.
    public const int DefaultMaxRelatedDecisions = 5;

    // Cosine-similarity cut-off for treating a past decision as "related".
    // 0.7 is permissive enough to surface genuinely related topics that are worded
    // differently, while still filtering out unrelated history.
    public const double DefaultSimilarityThreshold = 0.7;

    public int MaxRelatedDecisions { get; set; } = DefaultMaxRelatedDecisions;
    public double SimilarityThreshold { get; set; } = DefaultSimilarityThreshold;
}
