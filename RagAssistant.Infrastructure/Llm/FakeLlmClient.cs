using RagAssistant.Application.Interfaces;

namespace RagAssistant.Infrastructure.Llm
{
    public sealed class FakeLlmClient : ILlmClient
    {
        public string? LastPrompt { get; private set; }

        public Task<string> GenerateAsync(
            string prompt,
            CancellationToken cancellationToken = default)
        {
            LastPrompt = prompt;

            return Task.FromResult("Fake answer");
        }
    }
}
