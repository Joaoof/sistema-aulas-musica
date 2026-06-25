using MediatR;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Application.Features.Admin.Plans;

public record GetPlansQuery : IRequest<IReadOnlyList<PlanDto>>;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, IReadOnlyList<PlanDto>>
{
    private readonly IPlanRepository _plans;
    public GetPlansQueryHandler(IPlanRepository plans) => _plans = plans;

    public async Task<IReadOnlyList<PlanDto>> Handle(GetPlansQuery request, CancellationToken ct)
    {
        var plans = await _plans.GetAllAsync(ct);
        return plans.Select(PlanMapper.ToDto).ToList();
    }
}

public static class PlanMapper
{
    public static PlanDto ToDto(Domain.Entities.Plan p) => new(
        p.Id, p.Code, p.Name, p.SessionsPerMonth, p.DurationMinutes, p.Price, p.Summary,
        p.Features.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
