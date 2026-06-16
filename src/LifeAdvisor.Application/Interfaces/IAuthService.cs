using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default);
    Task DeleteUserAsync(string userId, CancellationToken ct = default);
}
