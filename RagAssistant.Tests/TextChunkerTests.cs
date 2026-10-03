using RagAssistant.Application.Models;
using RagAssistant.Infrastructure.Text;
using Xunit;

namespace RagAssistant.Tests
{
    public class TextChunkerTests
    {
        [Fact]
        public void Chunk_ShouldReturnSingleChunk_WhenTextIsShort()
        {
            // Arrange
            var options = new ChunkOptions
            {
                MaxChunkSize = 100,
                OverlapSize = 20
            };

            var chunker = new TextChunker(options);

            var text = "This is a short text.";

            // Act
            var result = chunker.Chunk(text);

            // Assert
            Assert.Single(result);
            Assert.Equal(text, result[0]);
        }

        [Fact]
        public void Chunk_ShouldCreateMultipleChunks_WhenTextIsLong()
        {
            // Arrange
            var options = new ChunkOptions
            {
                MaxChunkSize = 50,
                OverlapSize = 10
            };

            var chunker = new TextChunker(options);

            var text =
                "This is sentence one. " +
                "This is sentence two. " +
                "This is sentence three. " +
                "This is sentence four.";

            // Act
            var result = chunker.Chunk(text);

            // Assert
            Assert.True(result.Count > 1);
        }

        [Fact]
        public void Chunk_ShouldSplitLongSentence_WhenSentenceExceedsMaxChunkSize()
        {
            // Arrange
            var options = new ChunkOptions
            {
                MaxChunkSize = 50,
                OverlapSize = 10
            };

            var chunker = new TextChunker(options);

            var text =
                "This is a very long sentence that exceeds the maximum chunk size.";

            // Act
            var result = chunker.Chunk(text);

            // Assert
            Assert.True(result.Count > 1);

            Assert.All(
                result,
                chunk => Assert.True(chunk.Length <= options.MaxChunkSize));
        }

        [Fact]
        public void Chunk_ShouldHaveWordBasedOverlap_WhenCreatingMultipleChunks()
        {
            // Arrange
            var options = new ChunkOptions
            {
                MaxChunkSize = 60,
                OverlapSize = 20
            };

            var chunker = new TextChunker(options);

            var text =
                "First sentence contains some useful information. " +
                "Second sentence contains more useful information. " +
                "Third sentence contains additional useful information.";

            // Act
            var result = chunker.Chunk(text);

            // Assert
            Assert.True(result.Count > 1);

            Assert.Contains(
                "useful information.",
                result[1]);
        }

        [Fact]
        public void Chunk_ShouldNeverExceedMaxChunkSize_WhenOverlapIsApplied()
        {
            // Arrange
            var options = new ChunkOptions
            {
                MaxChunkSize = 60,
                OverlapSize = 20
            };

            var chunker = new TextChunker(options);

            var text =
                "First sentence contains some useful information. " +
                "Second sentence contains more useful information. " +
                "Third sentence contains additional useful information.";

            // Act
            var result = chunker.Chunk(text);

            // Assert
            Assert.NotEmpty(result);

            Assert.All(
                result,
                chunk => Assert.True(
                    chunk.Length <= options.MaxChunkSize,
                    $"Chunk length {chunk.Length} exceeds " +
                    $"MaxChunkSize {options.MaxChunkSize}."));
        }

        [Fact]
        public void Chunk_ShouldPreserveParagraphContent_WhenPossible()
        {
            // Arrange
            var options = new ChunkOptions
            {
                MaxChunkSize = 100,
                OverlapSize = 20
            };

            var chunker = new TextChunker(options);

            var text =
                "Customer can create a wallet. " +
                "The wallet can be charged." +
                "\n\n" +
                "Transactions are recorded for every operation. " +
                "Each transaction has an amount and type.";

            // Act
            var result = chunker.Chunk(text);

            // Assert
            Assert.NotEmpty(result);

            Assert.All(
                result,
                chunk => Assert.True(
                    chunk.Length <= options.MaxChunkSize,
                    $"Chunk length {chunk.Length} exceeds " +
                    $"MaxChunkSize {options.MaxChunkSize}."));

            Assert.Contains(
                result,
                chunk => chunk.Contains("Customer can create a wallet."));

            Assert.Contains(
                result,
                chunk => chunk.Contains("The wallet can be charged."));

            Assert.Contains(
                result,
                chunk => chunk.Contains("Transactions are recorded for every operation."));

            Assert.Contains(
                result,
                chunk => chunk.Contains("Each transaction has an amount and type."));
        }
    }
}
