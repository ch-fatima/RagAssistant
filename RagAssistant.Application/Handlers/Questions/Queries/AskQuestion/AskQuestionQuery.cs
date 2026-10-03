using MediatR;

namespace RagAssistant.Application.Handlers.Questions.Queries.AskQuestion
{
    public sealed record AskQuestionQuery(
    string Question,
    int TopK = 3) : IRequest<string>;
}
