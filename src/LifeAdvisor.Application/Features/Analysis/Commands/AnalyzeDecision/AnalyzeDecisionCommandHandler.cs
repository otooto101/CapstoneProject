using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Commands.AnalyzeDecision;

public class AnalyzeDecisionCommandHandler(IDecisionAnalysisService decisionAnalysisService)
    : IRequestHandler<AnalyzeDecisionCommand, DecisionAnalysisResult>
{
    public Task<DecisionAnalysisResult> Handle(AnalyzeDecisionCommand request, CancellationToken ct)
        => decisionAnalysisService.AnalyzeAsync(request.IdentityUserId, request.Prompt, ct);
}
