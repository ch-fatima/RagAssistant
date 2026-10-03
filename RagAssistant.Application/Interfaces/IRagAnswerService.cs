namespace RagAssistant.Application.Interfaces
{
    public interface IRagAnswerService
    {
        Task<string> AnswerAsync(
            string question,
            int topK = 3,
            CancellationToken cancellationToken = default);
    }
}
