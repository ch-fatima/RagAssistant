using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;

namespace RagAssistant.Infrastructure.Retrieval
{
    public sealed class RagContextBuilder : IRagContextBuilder
    {
        public string Build(
            IReadOnlyList<SearchResult> results)
        {
            if (results is null || results.Count == 0)
                return string.Empty;

            return string.Join(
                Environment.NewLine + Environment.NewLine,
                results.Select((result, index) =>
                    $"[Context {index + 1}]\n{result.Document.Text}"));
        }
    }
}
