using Microsoft.AspNetCore.Identity;


namespace LifeAdvisor.Persistence.Identity;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime RefreshTokenCreated { get; set; }
    public int MaxRelatedDecisions { get; set; } = 5;
    public double SimilarityThreshold { get; set; } = 0.75;
}