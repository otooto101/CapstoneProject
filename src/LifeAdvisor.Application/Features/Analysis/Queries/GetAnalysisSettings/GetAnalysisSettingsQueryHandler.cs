using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Queries.GetAnalysisSettings;

public class GetAnalysisSettingsQueryHandler(IAnalysisSettingsService analysisSettingsService)
    : IRequestHandler<GetAnalysisSettingsQuery, AnalysisSettingsDto>
{
    public async Task<AnalysisSettingsDto> Handle(GetAnalysisSettingsQuery request, CancellationToken ct)
    {
        var settings = await analysisSettingsService.GetSettingsAsync(request.IdentityUserId, ct);
        return new AnalysisSettingsDto(settings.MaxRelatedDecisions, settings.SimilarityThreshold);
    }
}
