using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Queries.GetDecisionHistory;

public record GetDecisionHistoryQuery(string IdentityUserId) : IRequest<IReadOnlyList<DecisionHistoryItem>>;