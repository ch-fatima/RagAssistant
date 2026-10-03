using MediatR;
using Microsoft.AspNetCore.Mvc;
using RagAssistant.Application.Handlers.Documents.Commands.IngestDocument;
using RagAssistant.Application.Handlers.Questions.Queries.AskQuestion;

namespace RagAssistant.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("documents")]
        public async Task<IActionResult> AddDocument(
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

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(
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
