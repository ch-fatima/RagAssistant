using RagAssistant.Application.Models;

namespace RagAssistant.Application.Interfaces
{
    public interface IRagContextBuilder
    {
        string Build(
            IReadOnlyList<SearchResult> results);
    }
}
