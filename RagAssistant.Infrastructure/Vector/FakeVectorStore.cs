using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;

namespace RagAssistant.Infrastructure.Vector
{
    public sealed class FakeVectorStore : IVectorStore
    {
        public List<VectorDocument> Documents { get; } = [];

        public Task AddAsync(
            VectorDocument document,
            CancellationToken cancellationToken = default)
        {
            Documents.Add(document);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(
            IEnumerable<VectorDocument> documents,
            CancellationToken cancellationToken = default)
        {
            Documents.AddRange(documents);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<SearchResult>> SearchAsync(
            float[] queryEmbedding,
            int topK,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
