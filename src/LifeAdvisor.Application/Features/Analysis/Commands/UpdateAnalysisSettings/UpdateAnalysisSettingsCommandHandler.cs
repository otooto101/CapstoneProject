using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Commands.UpdateAnalysisSettings;

public class UpdateAnalysisSettingsCommandHandler(IAnalysisSettingsService analysisSettingsService)
    : IRequestHandler<UpdateAnalysisSettingsCommand>
{
    public async Task Handle(UpdateAnalysisSettingsCommand request, CancellationToken ct)
    {
        if (request.MaxRelatedDecisions is < 1 or > 20)
            throw new InvalidOperationException("Max related decisions must be between 1 and 20.");

        if (request.SimilarityThreshold is < 0 or > 1)
            throw new InvalidOperationException("Similarity threshold must be between 0 and 1.");

        await analysisSettingsService.UpdateSettingsAsync(request.IdentityUserId, new AnalysisSettings
        {
            MaxRelatedDecisions = request.MaxRelatedDecisions,
            SimilarityThreshold = request.SimilarityThreshold
        }, ct);
    }
}
