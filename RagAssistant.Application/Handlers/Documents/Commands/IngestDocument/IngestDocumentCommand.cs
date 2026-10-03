using MediatR;

namespace RagAssistant.Application.Handlers.Documents.Commands.IngestDocument
{
    public sealed record IngestDocumentCommand(
    string Content) : IRequest;
}
