namespace RagAssistant.Application.Interfaces
{
    public interface ILlmClient
    {
        Task<string> GenerateAsync(
            string prompt,
            CancellationToken cancellationToken = default);
    }
}
