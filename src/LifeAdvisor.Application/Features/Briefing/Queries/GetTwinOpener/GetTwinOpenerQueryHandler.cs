using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Queries.GetTwinOpener;

public class GetTwinOpenerQueryHandler(ITwinChatService twinChatService)
    : IRequestHandler<GetTwinOpenerQuery, string>
{
    public async Task<string> Handle(GetTwinOpenerQuery request, CancellationToken ct)
        => await twinChatService.GetOpeningMessageAsync(request.IdentityUserId, ct);
}
