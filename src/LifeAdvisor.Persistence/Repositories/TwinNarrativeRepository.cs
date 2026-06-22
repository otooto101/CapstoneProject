using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAdvisor.Persistence.Repositories;

public class TwinNarrativeRepository(TwinDbContext context)
    : BaseRepository<TwinNarrative, TwinDbContext>(context), ITwinNarrativeRepository
{
    public async Task<IReadOnlyList<TwinNarrative>> ListByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
        => await context.TwinNarratives
            .Include(narrative => narrative.Twin)
            .Where(narrative => narrative.Twin.IdentityUserId == identityUserId)
            .OrderByDescending(narrative => narrative.CreatedAt)
            .ToListAsync(ct);

    public async Task<TwinNarrative?> GetDecisionByIdAsync(int twinNarrativeId, string identityUserId, CancellationToken ct = default)
        => await context.TwinNarratives
            .Include(narrative => narrative.Twin)
            .FirstOrDefaultAsync(
                narrative => narrative.TwinNarrativeId == twinNarrativeId
                    && narrative.Twin.IdentityUserId == identityUserId
                    && narrative.IsDecision,
                ct);

    public async Task<IReadOnlyList<TwinNarrative>> ListDecisionHistoryAsync(string identityUserId, CancellationToken ct = default)
        => await context.TwinNarratives
            .Include(narrative => narrative.Twin)
            .Where(narrative => narrative.Twin.IdentityUserId == identityUserId && narrative.IsDecision)
            .OrderByDescending(narrative => narrative.CreatedAt)
            .ToListAsync(ct);
}
