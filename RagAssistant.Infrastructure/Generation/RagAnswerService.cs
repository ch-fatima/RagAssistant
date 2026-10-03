using RagAssistant.Application.Interfaces;

namespace RagAssistant.Infrastructure.Generation
{
    public sealed class RagAnswerService : IRagAnswerService
    {
        private readonly IRagRetriever _retriever;
        private readonly IRagContextBuilder _contextBuilder;
        private readonly IRagPromptBuilder _promptBuilder;
        private readonly ILlmClient _llmClient;

        public RagAnswerService(
            IRagRetriever retriever,
            IRagContextBuilder contextBuilder,
            IRagPromptBuilder promptBuilder,
            ILlmClient llmClient)
        {
            _retriever = retriever;
            _contextBuilder = contextBuilder;
            _promptBuilder = promptBuilder;
            _llmClient = llmClient;
        }

        public async Task<string> AnswerAsync(
            string question,
            int topK = 3,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException(
                    "Question cannot be empty.",
                    nameof(question));

            if (topK <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(topK));

            var results = await _retriever.RetrieveAsync(
                question,
                topK,
                cancellationToken);

            var context = _contextBuilder.Build(results);

            if (string.IsNullOrWhiteSpace(context))
                return "I don't know based on the provided context.";

            var prompt = _promptBuilder.Build(
                question,
                context);

            return await _llmClient.GenerateAsync(
                prompt,
                cancellationToken);
        }
    }
}
