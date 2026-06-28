using LifeAdvisor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Application.Interfaces.Repositories
{
    public interface IDigitalTwinRepository : IRepository<DigitalTwin>
    {
    Task<DigitalTwin?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);

    // The twin with its life stage and selected life domains loaded, for building a
    // personalized AI prompt.
    Task<DigitalTwin?> GetWithProfileAsync(string identityUserId, CancellationToken ct = default);
    }
}
