namespace LifeAdvisor.Application.Models;

public class NewsSettings
{
    public string ApiKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://gnews.io/api/v4";
}
