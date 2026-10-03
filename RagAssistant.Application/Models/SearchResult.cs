namespace RagAssistant.Application.Models
{
    public sealed record SearchResult(
    VectorDocument Document,
    float Score);
}
