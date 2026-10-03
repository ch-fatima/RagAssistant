using OpenAI.Chat;
using RagAssistant.Application.Interfaces;

namespace RagAssistant.Infrastructure.Llm
{
    public sealed class OpenAiLlmClient : ILlmClient
    {
        private readonly ChatClient _client;

        public OpenAiLlmClient(ChatClient client)
        {
            _client = client;
        }

        public async Task<string> GenerateAsync(
            string prompt,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException(
                    "Prompt cannot be empty.",
                    nameof(prompt));

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            var result = await _client.CompleteChatAsync(
                messages,
                cancellationToken: cancellationToken);

            var completion = result.Value;

            return completion.Content[0].Text;
        }
    }
}
