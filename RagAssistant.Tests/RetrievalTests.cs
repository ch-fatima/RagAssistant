using RagAssistant.Application.Models;
using RagAssistant.Infrastructure.Embedding;
using RagAssistant.Infrastructure.Retrieval;
using RagAssistant.Infrastructure.Vector;
using Xunit;

namespace RagAssistant.Tests
{
    public class RetrievalTests
    {
        [Fact]
        public void Build_ShouldCombineSearchResultsIntoContext()
        {
            var results = new List<SearchResult>
            {
            new(
                new VectorDocument
                {
                    Id = Guid.NewGuid(),
                    Text = "Redis is an in-memory data store.",
                    Embedding = [1f, 0f]
                },
                0.91f),

            new(
                new VectorDocument
                {
                    Id = Guid.NewGuid(),
                    Text = "Redis supports caching.",
                    Embedding = [0.9f, 0.1f]
                },
                0.87f)
            };

            var builder = new RagContextBuilder();

            var context = builder.Build(results);

            Assert.Contains(
                "[Context 1]",
                context);

            Assert.Contains(
                "Redis is an in-memory data store.",
                context);

            Assert.Contains(
                "[Context 2]",
                context);

            Assert.Contains(
                "Redis supports caching.",
                context);
        }

        [Fact]
        public void Build_ShouldReturnEmpty_WhenThereAreNoResults()
        {
            var builder = new RagContextBuilder();

            var context = builder.Build([]);

            Assert.Equal(
                string.Empty,
                context);
        }

        [Fact]
        public async Task RetrieveAsync_ShouldReturnRelevantDocuments()
        {
            var embeddingGenerator =
                new FakeEmbeddingGenerator();

            var vectorStore =
                new InMemoryVectorStore();

            await vectorStore.AddRangeAsync(
            [
                new VectorDocument
        {
            Id = Guid.NewGuid(),
            Text = "Redis is an in-memory data store.",
            Embedding = [1f, 0f]
        },
        new VectorDocument
        {
            Id = Guid.NewGuid(),
            Text = "SQL Server is a relational database.",
            Embedding = [0f, 1f]
        }
            ]);

            var retriever = new RagRetriever(
                embeddingGenerator,
                vectorStore);

            var results = await retriever.RetrieveAsync(
                "What is Redis?",
                topK: 1);

            Assert.Single(results);

            Assert.Equal(
                "Redis is an in-memory data store.",
                results[0].Document.Text);
        }
    }
}
