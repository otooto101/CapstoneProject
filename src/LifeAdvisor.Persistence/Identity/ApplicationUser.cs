using Microsoft.AspNetCore.Identity;


namespace LifeAdvisor.Persistence.Identity;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime RefreshTokenCreated { get; set; }
}