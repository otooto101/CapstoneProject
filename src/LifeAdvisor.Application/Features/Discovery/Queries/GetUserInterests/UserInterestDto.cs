namespace LifeAdvisor.Application.Features.Discovery.Queries.GetUserInterests;

public record UserInterestDto(int TopicId, string Title, string Description, string Category, string Emoji, string ImageUrl, int Priority)
{
    public string PriorityLabel => Priority switch
    {
        >= 3 => "Obsessed",
        2 => "Keen",
        _ => "Curious"
    };
}
