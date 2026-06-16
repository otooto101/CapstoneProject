using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Persistence.Repositories
{
    public class DigitalTwinRepository(TwinDbContext context)
        : BaseRepository<DigitalTwin, TwinDbContext>(context), IDigitalTwinRepository
    {
    }
}
