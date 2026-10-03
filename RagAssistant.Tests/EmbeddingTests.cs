using RagAssistant.Infrastructure.Embedding;
using Xunit;

namespace RagAssistant.Tests
{
    public class EmbeddingTests
    {
        [Fact]
        public async Task GenerateAsync_ShouldReturnEmbedding()
        {
            var generator = new FakeEmbeddingGenerator();

            var result = await generator.GenerateAsync("Hello RAG");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GenerateAsync_ShouldThrow_WhenTextIsEmpty()
        {
            var generator = new FakeEmbeddingGenerator();

            await Assert.ThrowsAsync<ArgumentException>(
                () => generator.GenerateAsync(""));
        }
    }
}
