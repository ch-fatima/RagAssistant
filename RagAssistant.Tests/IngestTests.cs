using RagAssistant.Application.Models;
using RagAssistant.Infrastructure.Embedding;
using RagAssistant.Infrastructure.RagIngestion;
using RagAssistant.Infrastructure.Text;
using RagAssistant.Infrastructure.Vector;
using Xunit;

namespace RagAssistant.Tests
{
    public class IngestTests
    {
        [Fact]
        public async Task IngestAsync_ShouldChunkEmbedAndStoreDocument()
        {
            var options = new ChunkOptions
            {
                MaxChunkSize = 100,
                OverlapSize = 10
            };

            var chunker = new TextChunker(options);
            var embeddingGenerator = new FakeEmbeddingGenerator();
            var vectorStore = new FakeVectorStore();

            var service = new RagIngestionService(
                chunker,
                embeddingGenerator,
                vectorStore);

            var document = """
                   C# is a programming language.

                   .NET is a development platform.

                   ASP.NET Core is used to build web applications.
                   """;

            await service.IngestAsync(document);

            Assert.NotEmpty(vectorStore.Documents);

            Assert.All(
                vectorStore.Documents,
                document =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(document.Text));
                    Assert.NotEmpty(document.Embedding);
                    Assert.NotEqual(Guid.Empty, document.Id);
                });
        }

        [Fact]
        public async Task IngestAsync_ShouldThrow_WhenDocumentIsEmpty()
        {
            var options = new ChunkOptions
            {
                MaxChunkSize = 100,
                OverlapSize = 10
            };

            var service = new RagIngestionService(
                new TextChunker(options),
                new FakeEmbeddingGenerator(),
                new FakeVectorStore());

            await Assert.ThrowsAsync<ArgumentException>(
                () => service.IngestAsync(""));
        }

    }
}
