using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

public interface IOnboardingCacheService
{
    Task<string> CreateSessionAsync(OnboardingSessionState initialState, CancellationToken ct);
    Task UpdateSessionAsync(string sessionId, OnboardingSessionState updatedState, CancellationToken ct);
    Task<OnboardingSessionState?> GetSessionAsync(string sessionId, CancellationToken ct);
    Task DeleteSessionAsync(string sessionId, CancellationToken ct);
}