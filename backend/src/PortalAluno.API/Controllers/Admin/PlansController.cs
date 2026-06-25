using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalAluno.Application.Features.Admin;
using PortalAluno.Application.Features.Admin.Plans;

namespace PortalAluno.API.Controllers.Admin;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/admin/plans")]
public class PlansController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlansController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlanDto>>> List(CancellationToken ct)
        => Ok(await _mediator.Send(new GetPlansQuery(), ct));
}
