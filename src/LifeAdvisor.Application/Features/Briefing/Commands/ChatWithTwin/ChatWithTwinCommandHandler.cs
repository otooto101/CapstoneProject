using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Commands.ChatWithTwin;

public class ChatWithTwinCommandHandler(ITwinChatService twinChatService)
    : IRequestHandler<ChatWithTwinCommand, string>
{
    public async Task<string> Handle(ChatWithTwinCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new InvalidOperationException("Type a message so your twin can respond.");

        return await twinChatService.ReplyAsync(request.IdentityUserId, request.History, request.Message, ct);
    }
}
