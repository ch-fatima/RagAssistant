using MediatR;
using Microsoft.AspNetCore.Mvc;
using RagAssistant.Application.Handlers.Documents.Commands.IngestDocument;
using RagAssistant.Application.Handlers.Documents.Commands.IngestFile;
using RagAssistant.Application.Handlers.Questions.Queries.AskQuestion;

namespace RagAssistant.Api.Controllers
{

    /// <summary>
    /// RagController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RagController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Rag Constructor
        /// </summary>
        /// <param name="mediator"></param>
        public RagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Text To Vector InMemory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("AddDocument")]
        public async Task<IActionResult> AddDocumentAsync(
            [FromBody] IngestDocumentCommand request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new IngestDocumentCommand(request.Content),
                cancellationToken);

            return Ok(new
            {
                message = "Document indexed successfully."
            });
        }

        /// <summary>
        /// PdfFile To Vector InMemory By Ocr
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("AddDocumentPdfFile")]
        public async Task<IActionResult> AddDocumentPdfFileAsync(
            [FromForm] IngestPdfFileCommand request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new IngestPdfFileCommand(request.File),
                cancellationToken);

            return Ok(new
            {
                message = "PDF indexed successfully."
            });
        }

        /// <summary>
        /// Get Answer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("ask")]
        public async Task<IActionResult> AskAsync(
            [FromBody] AskQuestionQuery request,
            CancellationToken cancellationToken)
        {
            var answer = await _mediator.Send(
                new AskQuestionQuery(
                    request.Question,
                    request.TopK),
                cancellationToken);

            return Ok(new
            {
                question = request.Question,
                answer
            });
        }
    }
}
