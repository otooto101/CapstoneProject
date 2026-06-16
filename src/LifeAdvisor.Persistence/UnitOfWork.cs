using LifeAdvisor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Persistence
{
    public class UnitOfWork(TwinDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await context.SaveChangesAsync(ct);

        public void Dispose() => context.Dispose();
    }
}
