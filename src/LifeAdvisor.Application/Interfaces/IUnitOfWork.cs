using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
