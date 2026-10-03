using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;
using RagAssistant.Domain.Utility;

namespace RagAssistant.Infrastructure.Vector
{
    public sealed class InMemoryVectorStore : IVectorStore
    {
        private readonly List<VectorDocument> _documents = [];

        public Task AddAsync(
            VectorDocument document,
            CancellationToken cancellationToken = default)
        {
            _documents.Add(document);

            return Task.CompletedTask;
        }

        public Task AddRangeAsync(
            IEnumerable<VectorDocument> documents,
            CancellationToken cancellationToken = default)
        {
            _documents.AddRange(documents);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<SearchResult>> SearchAsync(
            float[] queryEmbedding,
            int topK,
            CancellationToken cancellationToken = default)
        {
            if (queryEmbedding.Length == 0)
                throw new ArgumentException(
                    "Query embedding cannot be empty.");

            if (topK <= 0)
                throw new ArgumentOutOfRangeException(nameof(topK));

            var results = _documents
                .Select(document => new SearchResult(
                    document,
                    VectorMath.CosineSimilarity(
                        queryEmbedding,
                        document.Embedding)))
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();

            return Task.FromResult<IReadOnlyList<SearchResult>>(results);
        }
    }
}
