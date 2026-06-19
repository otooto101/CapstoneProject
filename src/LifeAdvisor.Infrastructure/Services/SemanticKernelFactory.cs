using LifeAdvisor.Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace LifeAdvisor.Infrastructure.Services;

public class SemanticKernelFactory(IOptions<SemanticKernelSettings> options)
{
    private readonly SemanticKernelSettings _settings = options.Value;

    public Kernel CreateKernel()
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ChatModel) ||
            string.IsNullOrWhiteSpace(_settings.EmbeddingModel))
        {
            throw new InvalidOperationException("Semantic Kernel is not configured. Set SemanticKernel:ApiKey, ChatModel, and EmbeddingModel.");
        }

        var builder = Kernel.CreateBuilder();
        builder.AddGoogleAIGeminiChatCompletion(_settings.ChatModel, _settings.ApiKey, ParseApiVersion(_settings.ApiVersion));
        builder.AddGoogleAIEmbeddingGeneration(_settings.EmbeddingModel, _settings.ApiKey, ParseApiVersion(_settings.ApiVersion));

        return builder.Build();
    }

    private static GoogleAIVersion ParseApiVersion(string apiVersion)
        => Enum.TryParse<GoogleAIVersion>(apiVersion, ignoreCase: true, out var parsed)
            ? parsed
            : GoogleAIVersion.V1_Beta;
}
