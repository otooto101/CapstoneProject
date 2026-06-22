using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Commands.UpdateAnalysisSettings;

public record UpdateAnalysisSettingsCommand(
    string IdentityUserId,
    int MaxRelatedDecisions,
    double SimilarityThreshold) : IRequest;
