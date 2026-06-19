using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Interfaces.Repositories;

public interface ITwinNarrativeRepository : IRepository<TwinNarrative>
{
    Task<IReadOnlyList<TwinNarrative>> ListByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);
    Task<TwinNarrative?> GetDecisionByIdAsync(int twinNarrativeId, string identityUserId, CancellationToken ct = default);
    Task<IReadOnlyList<TwinNarrative>> ListDecisionHistoryAsync(string identityUserId, CancellationToken ct = default);
}
