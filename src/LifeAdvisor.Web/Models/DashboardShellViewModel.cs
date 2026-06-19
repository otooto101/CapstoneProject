namespace LifeAdvisor.Web.Models;

public class DashboardShellViewModel
{
    public bool IsSignedIn { get; set; }
    public bool IsOnboardingCompleted { get; set; }
    public string DisplayName { get; set; } = "Alex";
    public string Email { get; set; } = "alex@email.com";
    public string Initial { get; set; } = "A";
}
