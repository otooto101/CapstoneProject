using LifeAdvisor.Domain.Enums;

namespace LifeAdvisor.Application.Models;

public record RelatedDecisionMatch(int TwinNarrativeId, NarrativeType Type, string Content, double SimilarityScore);
