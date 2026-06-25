using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalAluno.Application.Features.Practice;

namespace PortalAluno.API.Controllers;

[ApiController]
[Authorize]
[Route("api/students/{studentId:guid}/practice")]
public class PracticeController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public PracticeController(IMediator mediator) => _mediator = mediator;

    public record LogPracticeRequest(int Bpm, DateOnly? Date);

    /// <summary>Registra uma sessão de prática (BPM) e invalida o cache do dashboard.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LogPracticeResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LogPracticeResult>> Log(
        Guid studentId, [FromBody] LogPracticeRequest request, CancellationToken ct)
    {
        if (!OwnsResource(studentId))
            return Forbid();

        var result = await _mediator.Send(
            new LogPracticeCommand(studentId, request.Bpm, request.Date), ct);

        return result is null
            ? NotFound(new { message = "Aluno não encontrado." })
            : StatusCode(StatusCodes.Status201Created, result);
    }
}
