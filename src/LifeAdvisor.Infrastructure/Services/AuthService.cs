using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace LifeAdvisor.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(string email, string password, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return AuthResult.Failure(result.Errors.Select(e => e.Description));

        return AuthResult.Success(user.Id);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
            return AuthResult.Failure(["Invalid email or password."]);

        var token = jwtService.GenerateToken(user.Id, email);

        return AuthResult.Success(user.Id, token);
    }

    public async Task DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is not null)
            await userManager.DeleteAsync(user);
    }
}
