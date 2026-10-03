using RagAssistant.Application.Interfaces;

namespace RagAssistant.Infrastructure.Embedding
{
    public sealed class FakeEmbeddingGenerator : IEmbeddingGenerator
    {
        public Task<float[]> GenerateAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException(
                    "Text cannot be empty.",
                    nameof(text));

            // فعلاً فقط برای تست
            return Task.FromResult(
                new float[] { 1f, 0f });
        }
    }
}
