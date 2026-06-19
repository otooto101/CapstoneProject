using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Infrastructure.Services;

public class RelatedDecisionRetriever(
    ITwinNarrativeRepository twinNarrativeRepository,
    ITextEmbeddingService textEmbeddingService) : IRelatedDecisionRetriever
{
    public async Task<IReadOnlyList<RelatedDecisionMatch>> RetrieveAsync(
        string identityUserId,
        ReadOnlyMemory<float> queryEmbedding,
        int maxRelatedDecisions,
        double similarityThreshold,
        CancellationToken ct = default)
    {
        var narratives = await twinNarrativeRepository.ListByIdentityUserIdAsync(identityUserId, ct);

        var results = new List<RelatedDecisionMatch>();

        foreach (var narrative in narratives)
        {
            var narrativeEmbedding = narrative.Embedding is not null
                ? ToFloatArray(narrative.Embedding!)
                : (await textEmbeddingService.GenerateAsync(narrative.Content, ct)).ToArray();

            var similarity = CosineSimilarity(queryEmbedding.Span, narrativeEmbedding.AsSpan());

            if (similarity < similarityThreshold)
                continue;

            results.Add(new RelatedDecisionMatch(
                narrative.TwinNarrativeId,
                narrative.Type,
                narrative.Content,
                similarity));
        }

        return results
            .OrderByDescending(match => match.SimilarityScore)
            .Take(maxRelatedDecisions)
            .ToList();
    }

    private static float[] ToFloatArray(dynamic embedding)
        => ((IEnumerable<float>)embedding).ToArray();

    private static double CosineSimilarity(ReadOnlySpan<float> left, ReadOnlySpan<float> right)
    {
        if (left.Length == 0 || right.Length == 0 || left.Length != right.Length)
            return 0;

        double dot = 0;
        double leftMagnitude = 0;
        double rightMagnitude = 0;

        for (var i = 0; i < left.Length; i++)
        {
            dot += left[i] * right[i];
            leftMagnitude += left[i] * left[i];
            rightMagnitude += right[i] * right[i];
        }

        if (leftMagnitude == 0 || rightMagnitude == 0)
            return 0;

        return dot / (Math.Sqrt(leftMagnitude) * Math.Sqrt(rightMagnitude));
    }
}
