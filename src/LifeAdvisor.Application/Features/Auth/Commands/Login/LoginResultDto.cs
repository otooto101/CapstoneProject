namespace LifeAdvisor.Application.Features.Auth.Commands.Login;

public record LoginResultDto(string UserId, string Email, string Token);
