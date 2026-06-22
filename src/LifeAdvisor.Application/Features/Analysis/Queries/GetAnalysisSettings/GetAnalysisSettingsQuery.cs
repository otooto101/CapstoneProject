using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Queries.GetAnalysisSettings;

public record GetAnalysisSettingsQuery(string IdentityUserId) : IRequest<AnalysisSettingsDto>;
