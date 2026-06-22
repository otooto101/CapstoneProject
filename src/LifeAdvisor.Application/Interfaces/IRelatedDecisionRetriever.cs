using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

public interface IRelatedDecisionRetriever
{
    Task<IReadOnlyList<RelatedDecisionMatch>> RetrieveAsync(
        string identityUserId,
        ReadOnlyMemory<float> queryEmbedding,
        int maxRelatedDecisions,
        double similarityThreshold,
        CancellationToken ct = default);
}
