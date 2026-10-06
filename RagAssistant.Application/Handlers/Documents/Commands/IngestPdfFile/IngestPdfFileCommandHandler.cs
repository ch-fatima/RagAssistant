using MediatR;
using RagAssistant.Application.Handlers.Documents.Commands.IngestDocument;
using RagAssistant.Application.Interfaces;

namespace RagAssistant.Application.Handlers.Documents.Commands.IngestFile
{
    public class IngestPdfFileCommandHandler : IRequestHandler<IngestPdfFileCommand>
    {
        private readonly IPdfTextExtractor  _pdfTextExtractor;
        private readonly IMediator _mediator;

        public IngestPdfFileCommandHandler(IPdfTextExtractor pdfTextExtractor, IMediator mediator)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _mediator = mediator;
        }

        public async Task Handle(IngestPdfFileCommand request, CancellationToken cancellationToken)
        {
            if (request.File is null || request.File.Length == 0)
            {
                throw new ArgumentException(
                    "PDF file cannot be empty.",
                    nameof(request.File));
            }

            if (!string.Equals(request.File.ContentType, 
                "application/pdf"
                ,StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Only PDF files are supported.",
                    nameof(request.File));
            }

            await using var stream = request.File.OpenReadStream();
            var text = await _pdfTextExtractor.ExtractTextAsync(stream, cancellationToken);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("No readable text was found in the PDF.",
                    nameof(request.File));
            }

            await _mediator.Send(new IngestDocumentCommand(text), cancellationToken);
        }
    }
}
