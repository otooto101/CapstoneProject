using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Commands.AnalyzeDecision;

public record AnalyzeDecisionCommand(string IdentityUserId, string Prompt) : IRequest<DecisionAnalysisResult>;
