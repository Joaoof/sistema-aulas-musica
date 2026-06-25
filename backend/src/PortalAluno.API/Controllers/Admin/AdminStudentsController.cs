using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalAluno.Application.Features.Admin;
using PortalAluno.Application.Features.Admin.Students;

namespace PortalAluno.API.Controllers.Admin;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/admin/students")]
public class AdminStudentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminStudentsController(IMediator mediator) => _mediator = mediator;

    public record CreateStudentRequest(string Name, string Email, string Instrument);
    public record AssignPlanRequest(Guid PlanId, decimal? MonthlyPrice, int? MonthlySessions);
    public record AddRepertoireRequest(string Title, string Composer, string? VideoUrl, string? Status);
    public record AddMaterialRequest(string Title, string Type, string ExternalUrl);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentSummaryDto>>> List(CancellationToken ct)
        => Ok(await _mediator.Send(new GetStudentsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDetailDto>> Detail(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetStudentDetailQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<StudentSummaryDto>> Create(
        [FromBody] CreateStudentRequest r, CancellationToken ct)
    {
        var dto = await _mediator.Send(new CreateStudentCommand(r.Name, r.Email, r.Instrument), ct);
        return CreatedAtAction(nameof(Detail), new { id = dto.Id }, dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _mediator.Send(new DeleteStudentCommand(id), ct) ? NoContent() : NotFound();

    [HttpPut("{id:guid}/plan")]
    public async Task<ActionResult<AssignedPlanDto>> AssignPlan(
        Guid id, [FromBody] AssignPlanRequest r, CancellationToken ct)
    {
        var dto = await _mediator.Send(
            new AssignPlanCommand(id, r.PlanId, r.MonthlyPrice, r.MonthlySessions), ct);
        return dto is null ? NotFound(new { message = "Aluno ou plano não encontrado." }) : Ok(dto);
    }

    [HttpPost("{id:guid}/repertoire")]
    public async Task<IActionResult> AddRepertoire(
        Guid id, [FromBody] AddRepertoireRequest r, CancellationToken ct)
    {
        var ok = await _mediator.Send(
            new AddRepertoireCommand(id, r.Title, r.Composer, r.VideoUrl, r.Status), ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/materials")]
    public async Task<IActionResult> AddMaterial(
        Guid id, [FromBody] AddMaterialRequest r, CancellationToken ct)
    {
        var ok = await _mediator.Send(new AddMaterialCommand(id, r.Title, r.Type, r.ExternalUrl), ct);
        return ok ? NoContent() : NotFound();
    }
}
