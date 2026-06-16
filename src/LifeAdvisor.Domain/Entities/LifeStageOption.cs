using System.Collections.Generic;

namespace LifeAdvisor.Domain.Entities;

public class LifeStageOption
{
    public int LifeStageOptionId { get; set; }

    // e.g., "Student / Early career", "Settling down / Starting family"
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public ICollection<DigitalTwin> Twins { get; set; } = new List<DigitalTwin>();
}