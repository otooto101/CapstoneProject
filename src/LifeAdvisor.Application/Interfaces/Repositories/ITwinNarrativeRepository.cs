using LifeAdvisor.Application.Models;
using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Interfaces.Repositories;

public interface ITwinNarrativeRepository : IRepository<TwinNarrative>
{
    Task<IReadOnlyList<TwinNarrative>> ListByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);
    Task<TwinNarrative?> GetDecisionByIdAsync(int twinNarrativeId, string identityUserId, CancellationToken ct = default);
    Task<IReadOnlyList<TwinNarrative>> ListDecisionHistoryAsync(string identityUserId, CancellationToken ct = default);

    Task<(IReadOnlyList<DecisionHistoryRow> Rows, int TotalCount)> SearchDecisionHistoryAsync(
        string identityUserId,
        string? search,
        bool? isCompleted,
        int page,
        int pageSize,
        CancellationToken ct = default);
}
