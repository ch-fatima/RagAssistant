namespace RagAssistant.Application.Interfaces
{
    public interface IPdfOcrExtractor
    {
        Task<string> ExtractTextAsync(
            Stream pdfStream,
            CancellationToken cancellationToken = default);
    }
}
