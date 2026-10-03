using OpenAI.Embeddings;
using RagAssistant.Application.Interfaces;
using System.ClientModel;

namespace RagAssistant.Infrastructure.Embedding
{
    public sealed class OpenAIEmbeddingGenerator : IEmbeddingGenerator 
    { 
        private readonly EmbeddingClient _client; 
        public OpenAIEmbeddingGenerator(EmbeddingClient client) 
        { 
            _client = client; 
        } 
        public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default) 
        { 
            if (string.IsNullOrWhiteSpace(text)) 
                throw new ArgumentException("Text cannot be empty.", nameof(text)); 
            ClientResult<OpenAIEmbedding> result = await _client.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken); 
            return result.Value.ToFloats().ToArray(); 
        } 
    }
}
