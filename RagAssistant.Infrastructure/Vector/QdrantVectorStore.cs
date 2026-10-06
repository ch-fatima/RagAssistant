using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace RagAssistant.Infrastructure.Vector
{
    public sealed class QdrantVectorStore : IVectorStore
    {
        private const string CollectionName = "rag_documents";

        private readonly QdrantClient _client;

        public QdrantVectorStore(QdrantClient client)
        {
            _client = client;
        }

        public async Task AddAsync(
            VectorDocument document,
            CancellationToken cancellationToken = default)
        {
            await EnsureCollectionAsync(
                document.Embedding.Length,
                cancellationToken);

            var point = new PointStruct
            {
                Id = new PointId
                {
                    Uuid = document.Id.ToString()
                },
                Vectors = document.Embedding,
                Payload =
                {
                    ["text"] = document.Text
                }
            };

            foreach (var metadata in document.Metadata)
            {
                point.Payload[metadata.Key] = metadata.Value;
            }

            await _client.UpsertAsync(
                CollectionName,
                new[] { point },
                cancellationToken: cancellationToken);
        }

        public async Task AddRangeAsync(
            IEnumerable<VectorDocument> documents,
            CancellationToken cancellationToken = default)
        {
            var items = documents.ToList();

            if (items.Count == 0)
                return;

            await EnsureCollectionAsync(
                items[0].Embedding.Length,
                cancellationToken);

            var points = items.Select(document =>
            {
                var point = new PointStruct
                {
                    Id = new PointId
                    {
                        Uuid = document.Id.ToString()
                    },
                    Vectors = document.Embedding,
                    Payload =
                    {
                        ["text"] = document.Text
                    }
                };

                foreach (var metadata in document.Metadata)
                {
                    point.Payload[metadata.Key] = metadata.Value;
                }

                return point;

            }).ToList();

            await _client.UpsertAsync(
                CollectionName,
                points,
                cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<SearchResult>> SearchAsync(
            float[] queryEmbedding,
            int topK,
            CancellationToken cancellationToken = default)
        {
            if (queryEmbedding.Length == 0)
                throw new ArgumentException(
                    "Query embedding cannot be empty.");

            if (topK <= 0)
                throw new ArgumentOutOfRangeException(nameof(topK));

            var results = await _client.QueryAsync(
                collectionName: CollectionName,
                query: queryEmbedding,
                limit: (ulong)topK,
                cancellationToken: cancellationToken);

            return results
                .Select(result =>
                {
                    var text = result.Payload.TryGetValue(
                        "text",
                        out var textValue)
                        ? textValue.StringValue
                        : string.Empty;

                    var document = new VectorDocument
                    {
                        Id = Guid.Parse(result.Id.Uuid),
                        Text = text,
                        Embedding = [],
                        Metadata = result.Payload
                            .Where(x => x.Key != "text")
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value.StringValue)
                    };

                    return new SearchResult(
                        document,
                        result.Score);
                })
                .ToList();
        }

        private async Task EnsureCollectionAsync(
            int vectorSize,
            CancellationToken cancellationToken)
        {
            var exists = await _client.CollectionExistsAsync(
                CollectionName,
                cancellationToken);

            if (exists)
                return;

            await _client.CreateCollectionAsync(
                CollectionName,
                new VectorParams
                {
                    Size = (ulong)vectorSize,
                    Distance = Distance.Cosine
                },
                cancellationToken: cancellationToken);
        }
    }
}
