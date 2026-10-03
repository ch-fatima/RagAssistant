using Microsoft.Extensions.Configuration;
using RagAssistant.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RagAssistant.Infrastructure.Llm
{
    public sealed class LocalLlmClient : ILlmClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public LocalLlmClient(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;

            _model =
                configuration["Ollama:Model"]
                ?? "qwen2.5:3b";
        }

        public async Task<string> GenerateAsync(
            string prompt,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException(
                    "Prompt cannot be empty.",
                    nameof(prompt));
            }

            var request = new OllamaRequest
            {
                Model = _model,
                Prompt = prompt,
                Stream = false
            };

            using var response =
                await _httpClient.PostAsJsonAsync(
                    "/api/generate",
                    request,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content.ReadFromJsonAsync<OllamaResponse>(
                    cancellationToken: cancellationToken);

            return result?.Response
                ?? throw new InvalidOperationException(
                    "Ollama returned an empty response.");
        }

        private sealed class OllamaRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; init; } = string.Empty;

            [JsonPropertyName("prompt")]
            public string Prompt { get; init; } = string.Empty;

            [JsonPropertyName("stream")]
            public bool Stream { get; init; }
        }

        private sealed class OllamaResponse
        {
            [JsonPropertyName("response")]
            public string Response { get; init; } = string.Empty;
        }
    }
}
