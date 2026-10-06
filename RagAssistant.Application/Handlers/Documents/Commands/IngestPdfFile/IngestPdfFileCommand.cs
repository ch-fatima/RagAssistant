using MediatR;
using Microsoft.AspNetCore.Http;

namespace RagAssistant.Application.Handlers.Documents.Commands.IngestFile
{
    public sealed record IngestPdfFileCommand(IFormFile File) : IRequest;
}
