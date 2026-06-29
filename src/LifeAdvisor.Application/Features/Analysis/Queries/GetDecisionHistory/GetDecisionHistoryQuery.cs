using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Queries.GetDecisionHistory;

public record GetDecisionHistoryQuery(
    string IdentityUserId,
    string? Search = null,
    bool? IsCompleted = null,
    int Page = 1,
    int PageSize = 10) : IRequest<DecisionHistoryPage>;