using RagAssistant.Application.Models;

namespace RagAssistant.Application.Interfaces
{
    public interface IVectorStore
    {
        Task AddAsync(
        VectorDocument document,
        CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<VectorDocument> documents,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SearchResult>> SearchAsync(
            float[] queryEmbedding,
            int topK,
            CancellationToken cancellationToken = default);
    }
}
