namespace LifeAdvisor.Application.Interfaces;

public interface ITextEmbeddingService
{
    Task<ReadOnlyMemory<float>> GenerateAsync(string text, CancellationToken ct = default);
}
