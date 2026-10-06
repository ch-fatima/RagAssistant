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
            You are a question-answering assistant.

            Your task is to answer the question based strictly on the provided context.

            Rules:
            1. Use only information explicitly available in the context.
            2. Do not use your own knowledge or make assumptions.
            3. Extract all relevant details from the context.
            4. Give a clear and complete answer.
            5. If multiple context sections contain relevant information, combine them.
            6. If the context does not contain enough information to answer the question, say:
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
