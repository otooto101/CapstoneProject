namespace LifeAdvisor.Application.Models;

public class SemanticKernelSettings
{
    public string ApiKey { get; init; } = string.Empty;
    public string ChatModel { get; init; } = string.Empty;
    public string EmbeddingModel { get; init; } = string.Empty;
    public string ApiVersion { get; init; } = "V1_Beta";
}
