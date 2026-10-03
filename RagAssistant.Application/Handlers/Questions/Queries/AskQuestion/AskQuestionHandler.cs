using MediatR;
using RagAssistant.Application.Interfaces;

namespace RagAssistant.Application.Handlers.Questions.Queries.AskQuestion
{
    public sealed class AskQuestionHandler
    : IRequestHandler<AskQuestionQuery, string>
    {
        private readonly IRagAnswerService _answerService;

        public AskQuestionHandler(
            IRagAnswerService answerService)
        {
            _answerService = answerService;
        }

        public async Task<string> Handle(
            AskQuestionQuery request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                throw new ArgumentException(
                    "Question cannot be empty.",
                    nameof(request.Question));

            return await _answerService.AnswerAsync(
                request.Question,
                request.TopK,
                cancellationToken);
        }
    }
}
