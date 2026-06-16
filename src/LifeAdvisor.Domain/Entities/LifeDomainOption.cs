using System.Collections.Generic;

namespace LifeAdvisor.Domain.Entities;

public class LifeDomainOption
{
    public int LifeDomainOptionId { get; set; }

    // e.g., "Health & Body", "Career & Purpose"
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    // Navigation back to the join table
    public ICollection<TwinLifeDomain> SelectedByTwins { get; set; } = new List<TwinLifeDomain>();
}