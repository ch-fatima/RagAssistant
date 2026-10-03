namespace RagAssistant.Application.Models
{
    public sealed class VectorDocument
    {
        public Guid Id { get; init; }

        public string Text { get; init; } = string.Empty;

        public float[] Embedding { get; init; } = [];

        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
