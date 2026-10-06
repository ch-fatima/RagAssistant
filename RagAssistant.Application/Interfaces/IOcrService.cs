namespace RagAssistant.Application.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractTextAsync(
            Stream imageStream,
            string language = "fas+eng",
            CancellationToken cancellationToken = default);
    }
}
