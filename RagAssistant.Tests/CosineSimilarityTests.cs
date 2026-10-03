using RagAssistant.Application.Models;
using RagAssistant.Domain.Utility;
using RagAssistant.Infrastructure.Vector;
using Xunit;

namespace RagAssistant.Tests
{
    public class CosineSimilarityTests
    {
        [Fact]
        public void CosineSimilarity_ShouldReturnOne_ForIdenticalVectors()
        {
            var vector = new float[] { 1, 2, 3 };

            var result = VectorMath.CosineSimilarity(vector, vector);

            Assert.Equal(1f, result, precision: 5);
        }

        [Fact]
        public void CosineSimilarity_ShouldReturnZero_ForOrthogonalVectors()
        {
            var a = new float[] { 1, 0 };
            var b = new float[] { 0, 1 };

            var result = VectorMath.CosineSimilarity(a, b);

            Assert.Equal(0f, result, precision: 5);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMostSimilarDocuments()
        {
            var store = new InMemoryVectorStore();

            await store.AddRangeAsync(
            [
                new VectorDocument
        {
            Id = Guid.NewGuid(),
            Text = "Redis is an in-memory data store",
            Embedding = [1, 0]
        },
        new VectorDocument
        {
            Id = Guid.NewGuid(),
            Text = "SQL Server is a relational database",
            Embedding = [0, 1]
        }
            ]);

            var query = new float[] { 0.9f, 0.1f };

            var results = await store.SearchAsync(query, topK: 1);

            Assert.Single(results);
            Assert.Equal(
                "Redis is an in-memory data store",
                results[0].Document.Text);
        }
    }
}
