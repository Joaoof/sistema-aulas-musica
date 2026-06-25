using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalAluno.Application.Features.Admin;
using PortalAluno.Application.Features.Admin.Lessons;

namespace PortalAluno.API.Controllers.Admin;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/admin/lessons")]
public class AdminLessonsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminLessonsController(IMediator mediator) => _mediator = mediator;

    public record ScheduleRequest(Guid StudentId, DateTime ScheduledAt, int DurationMinutes);
    public record JustifyRequest(string Reason);

    /// <summary>Checklist do dia — aulas agendadas para hoje (todos os alunos).</summary>
    [HttpGet("today")]
    public async Task<ActionResult<IReadOnlyList<LessonDto>>> Today(CancellationToken ct)
        => Ok(await _mediator.Send(new GetTodayLessonsQuery(), ct));

    [HttpGet("stats")]
    public async Task<ActionResult<IReadOnlyList<StudentSummaryDto>>> Stats(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return Ok(await _mediator.Send(new GetMonthlyStatsQuery(year ?? now.Year, month ?? now.Month), ct));
    }

    [HttpPost]
    public async Task<ActionResult<LessonDto>> Schedule([FromBody] ScheduleRequest r, CancellationToken ct)
    {
        var dto = await _mediator.Send(
            new ScheduleLessonCommand(r.StudentId, r.ScheduledAt, r.DurationMinutes), ct);
        return dto is null ? NotFound(new { message = "Aluno não encontrado." }) : Ok(dto);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
        => await _mediator.Send(new CompleteLessonCommand(id), ct) ? NoContent() : NotFound();

    [HttpPost("{id:guid}/justify")]
    public async Task<IActionResult> Justify(Guid id, [FromBody] JustifyRequest r, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.Reason))
            return BadRequest(new { message = "Justificativa é obrigatória." });
        return await _mediator.Send(new JustifyLessonCommand(id, r.Reason), ct) ? NoContent() : NotFound();
    }
}
