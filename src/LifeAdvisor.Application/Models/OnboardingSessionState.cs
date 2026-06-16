
namespace LifeAdvisor.Application.Models;

public class OnboardingSessionState
{
    public int LastCompletedStep { get; set; } = 0;
    // Step 0 Data (Auth)
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Step 1 Data (About you)
    public string? PreferredName { get; set; }
    public int? Age { get; set; } // Or DateOnly? DateOfBirth
    public string? LocationCity { get; set; }
    public string? LocationCountry { get; set; }
    public int? LifeStageOptionId { get; set; }

    // Step 2 Data (Your story)
    public string? DefiningMoment { get; set; }
    public string? HardestDecision { get; set; }

    // Step 3 Data (How you think)
    public string? DecisionStyle { get; set; }
    public string? UnderPressure { get; set; }

    // Step 4 Data (What matters)
    public List<int> SelectedLifeDomainIds { get; set; } = [];
    public string? ValuesContext { get; set; }

    // Step 5 Data (Where you're going)
    public string? LifeVision { get; set; }
    public string? CurrentStruggles { get; set; }
    public string? DeepestFears { get; set; }
}