using RagAssistant.Application.Models;

namespace RagAssistant.Application.Interfaces
{
    public interface IRagRetriever
    {
        Task<IReadOnlyList<SearchResult>> RetrieveAsync(
            string query,
            int topK = 3,
            CancellationToken cancellationToken = default);
    }
}
