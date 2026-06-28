using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Queries.GetTwinOpener;

// The twin's opening line for the interactive chat — the AI starts the conversation.
public record GetTwinOpenerQuery(string IdentityUserId) : IRequest<string>;
