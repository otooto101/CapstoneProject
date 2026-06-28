using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Interfaces.Repositories;

public interface IDailyBriefingRepository : IRepository<DailyBriefing>
{
    // Today's briefing for a twin (with its items), or null if not generated yet.
    Task<DailyBriefing?> GetForTwinOnDateAsync(int digitalTwinId, DateOnly date, CancellationToken ct = default);
}
