using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAdvisor.Persistence.Repositories;

public class DailyBriefingRepository(TwinDbContext context)
    : BaseRepository<DailyBriefing, TwinDbContext>(context), IDailyBriefingRepository
{
    public async Task<DailyBriefing?> GetForTwinOnDateAsync(int digitalTwinId, DateOnly date, CancellationToken ct = default)
        => await context.DailyBriefings
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.DigitalTwinId == digitalTwinId && b.BriefingDate == date, ct);
}
