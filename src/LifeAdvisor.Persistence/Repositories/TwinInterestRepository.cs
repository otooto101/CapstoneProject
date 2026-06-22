using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAdvisor.Persistence.Repositories;

public class TwinInterestRepository(TwinDbContext context)
    : BaseRepository<TwinInterest, TwinDbContext>(context), ITwinInterestRepository
{
    public async Task<TwinInterest?> GetByTwinAndTopicAsync(int digitalTwinId, int interestTopicId, CancellationToken ct = default)
        => await context.TwinInterests
            .FirstOrDefaultAsync(i => i.DigitalTwinId == digitalTwinId && i.InterestTopicId == interestTopicId, ct);

    public async Task<IReadOnlyList<TwinInterest>> ListInterestedByUserAsync(string identityUserId, CancellationToken ct = default)
        => await context.TwinInterests
            .Include(i => i.Topic)
            .Include(i => i.Twin)
            .Where(i => i.Twin.IdentityUserId == identityUserId && i.IsInterested)
            .OrderByDescending(i => i.Priority)
            .ThenByDescending(i => i.UpdatedAt ?? i.CreatedAt)
            .ToListAsync(ct);
}
