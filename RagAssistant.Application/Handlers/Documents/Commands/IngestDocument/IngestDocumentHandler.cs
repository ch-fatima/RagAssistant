using MediatR;
using RagAssistant.Application.Interfaces;

namespace RagAssistant.Application.Handlers.Documents.Commands.IngestDocument;

public sealed class IngestDocumentHandler
    : IRequestHandler<IngestDocumentCommand>
{
    private readonly IRagIngestionService _ingestionService;

    public IngestDocumentHandler(
        IRagIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task Handle(
        IngestDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new ArgumentException(
                "Document content cannot be empty.",
                nameof(request.Content));
        }

        await _ingestionService.IngestAsync(
            request.Content);
    }
}