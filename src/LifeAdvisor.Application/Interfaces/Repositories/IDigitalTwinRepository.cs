using LifeAdvisor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Application.Interfaces.Repositories
{
    public interface IDigitalTwinRepository : IRepository<DigitalTwin>
    {
    Task<DigitalTwin?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);
    }
}
