namespace LifeAdvisor.Application.Models;

public record AuthResult(bool Succeeded, IEnumerable<string> Errors, string? UserId = null, string? Token = null)
{
    public static AuthResult Success(string userId, string? token = null) => new(true, [], userId, token);
    public static AuthResult Failure(IEnumerable<string> errors) => new(false, errors);
}
