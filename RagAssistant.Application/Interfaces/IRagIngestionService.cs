namespace RagAssistant.Application.Interfaces
{
    public interface IRagIngestionService
    {
        Task IngestAsync(
            string document,
            CancellationToken cancellationToken = default);
    }
}
