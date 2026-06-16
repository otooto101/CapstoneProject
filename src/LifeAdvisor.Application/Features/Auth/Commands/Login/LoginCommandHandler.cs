using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, string>
{
    public async Task<string> Handle(LoginCommand request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, ct);

        if (!result.Succeeded)
            throw new UnauthorizedAccessException(string.Join("; ", result.Errors));

        return result.Token!;
    }
}
