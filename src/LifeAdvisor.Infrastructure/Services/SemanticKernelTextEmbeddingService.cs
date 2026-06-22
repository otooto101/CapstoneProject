using LifeAdvisor.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Embeddings;

namespace LifeAdvisor.Infrastructure.Services;

public class SemanticKernelTextEmbeddingService(SemanticKernelFactory semanticKernelFactory) : ITextEmbeddingService
{
    public Task<ReadOnlyMemory<float>> GenerateAsync(string text, CancellationToken ct = default)
    {
        var kernel = semanticKernelFactory.CreateKernel();
        var embeddingService = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();

        return embeddingService.GenerateEmbeddingAsync(text, cancellationToken: ct);
    }
}
