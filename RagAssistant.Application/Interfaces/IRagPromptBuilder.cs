namespace RagAssistant.Application.Interfaces
{
    public interface IRagPromptBuilder
    {
        string Build(
            string question,
            string context);
    }
}
