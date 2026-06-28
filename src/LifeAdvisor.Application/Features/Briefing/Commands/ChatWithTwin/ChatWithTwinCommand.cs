using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Commands.ChatWithTwin;

public record ChatWithTwinCommand(
    string IdentityUserId,
    IReadOnlyList<ChatTurn> History,
    string Message) : IRequest<string>;
