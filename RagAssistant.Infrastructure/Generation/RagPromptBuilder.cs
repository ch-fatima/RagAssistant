using RagAssistant.Application.Interfaces;

namespace RagAssistant.Infrastructure.Generation
{
    public sealed class RagPromptBuilder : IRagPromptBuilder
    {
        public string Build(
            string question,
            string context)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException(
                    "Question cannot be empty.",
                    nameof(question));

            if (string.IsNullOrWhiteSpace(context))
                throw new ArgumentException(
                    "Context cannot be empty.",
                    nameof(context));

            return $"""
                You are a helpful assistant.

                Answer the user's question using only the provided context.

                If the answer cannot be found in the context, say:
                "I don't know based on the provided context."

                Context:
                {context}

                Question:
                {question}

                Answer:
                """;
        }
    }
}
