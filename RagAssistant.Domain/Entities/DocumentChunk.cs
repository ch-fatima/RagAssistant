using RagAssistant.Domain.Common;

namespace RagAssistant.Domain.Entities
{
    public class DocumentChunk : BaseEntity
    {
        public Guid DocumentId { get; private set; }

        public string Content { get; private set; }

        public int ChunkIndex { get; private set; }

        public Document Document { get; private set; }

        private DocumentChunk()
        {
        }

        public DocumentChunk(Guid documentId, string content, int chunkIndex)
        {
            DocumentId = documentId;
            Content = content;
            ChunkIndex = chunkIndex;
        }
    }
}
