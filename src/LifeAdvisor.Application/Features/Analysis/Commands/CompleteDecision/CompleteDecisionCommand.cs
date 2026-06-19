using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Commands.CompleteDecision;

public record CompleteDecisionCommand(string IdentityUserId, int DecisionHistoryId, string SelectedScenarioTitle) : IRequest;