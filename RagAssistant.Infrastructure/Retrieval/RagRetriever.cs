using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;

namespace RagAssistant.Infrastructure.Retrieval
{
    public sealed class RagRetriever : IRagRetriever
    {
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IVectorStore _vectorStore;

        public RagRetriever(
            IEmbeddingGenerator embeddingGenerator,
            IVectorStore vectorStore)
        {
            _embeddingGenerator = embeddingGenerator;
            _vectorStore = vectorStore;
        }

        public async Task<IReadOnlyList<SearchResult>> RetrieveAsync(
            string query,
            int topK = 3,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException(
                    "Query cannot be empty.",
                    nameof(query));

            if (topK <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(topK));

            var queryEmbedding =
                await _embeddingGenerator.GenerateAsync(
                    query,
                    cancellationToken);

            return await _vectorStore.SearchAsync(
                queryEmbedding,
                topK,
                cancellationToken);
        }
    }
}
