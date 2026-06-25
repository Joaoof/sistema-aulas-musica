using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalAluno.Application.Features.Dashboard;

namespace PortalAluno.API.Controllers;

[ApiController]
[Authorize]
[Route("api/students/{studentId:guid}/dashboard")]
public class DashboardController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Dashboard consolidado do aluno (servido a partir do cache Redis quando disponível).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(StudentDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDashboardDto>> Get(Guid studentId, CancellationToken ct)
    {
        if (!OwnsResource(studentId))
            return Forbid();

        var dashboard = await _mediator.Send(new GetStudentDashboardQuery(studentId), ct);
        return dashboard is null
            ? NotFound(new { message = "Aluno não encontrado." })
            : Ok(dashboard);
    }
}
