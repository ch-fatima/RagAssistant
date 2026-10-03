using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;

namespace RagAssistant.Infrastructure.RagIngestion
{
    public sealed class RagIngestionService : IRagIngestionService
    {
        private readonly ITextChunker _textChunker;
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IVectorStore _vectorStore;

        public RagIngestionService(
            ITextChunker textChunker,
            IEmbeddingGenerator embeddingGenerator,
            IVectorStore vectorStore)
        {
            _textChunker = textChunker;
            _embeddingGenerator = embeddingGenerator;
            _vectorStore = vectorStore;
        }

        public async Task IngestAsync(
            string document,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException(
                    "Document cannot be empty.",
                    nameof(document));

            var chunks = _textChunker.Chunk(document);

            foreach (var chunk in chunks)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var embedding =
                    await _embeddingGenerator.GenerateAsync(
                        chunk,
                        cancellationToken);

                var vectorDocument = new VectorDocument
                {
                    Id = Guid.NewGuid(),
                    Text = chunk,
                    Embedding = embedding
                };

                await _vectorStore.AddAsync(
                    vectorDocument,
                    cancellationToken);
            }
        }
    }
}
