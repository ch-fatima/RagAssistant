namespace RagAssistant.Application.Models
{
    public class ChunkOptions
    {
        public int MaxChunkSize { get; set; } = 1000;

        public int OverlapSize { get; set; } = 150;
    }
}
