using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Persistence.Repositories;

public class LifeStageOptionRepository(TwinDbContext context)
    : BaseRepository<LifeStageOption, TwinDbContext>(context), ILifeStageOptionRepository
{
}
