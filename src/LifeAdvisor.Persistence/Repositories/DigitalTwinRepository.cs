using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Persistence.Repositories
{
    public class DigitalTwinRepository(TwinDbContext context)
        : BaseRepository<DigitalTwin, TwinDbContext>(context), IDigitalTwinRepository
    {
        public async Task<DigitalTwin?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
            => await context.DigitalTwins
                .FirstOrDefaultAsync(twin => twin.IdentityUserId == identityUserId, ct);
    }
}
