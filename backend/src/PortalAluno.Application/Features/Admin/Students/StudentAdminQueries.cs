using MediatR;
using PortalAluno.Application.Features.Dashboard;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Application.Features.Admin.Students;

// ── Lista de alunos (com plano + aulas feitas no mês) ─────────
public record GetStudentsQuery : IRequest<IReadOnlyList<StudentSummaryDto>>;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, IReadOnlyList<StudentSummaryDto>>
{
    private readonly IStudentRepository _students;
    private readonly ILessonRepository _lessons;

    public GetStudentsQueryHandler(IStudentRepository students, ILessonRepository lessons)
    {
        _students = students;
        _lessons = lessons;
    }

    public async Task<IReadOnlyList<StudentSummaryDto>> Handle(GetStudentsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var students = await _students.GetAllWithPlanAsync(ct);
        var counts = await _lessons.CountDoneByStudentInMonthAsync(now.Year, now.Month, ct);

        return students.Select(s => new StudentSummaryDto(
            s.Id, s.Name, s.Email, s.Instrument,
            s.Plan?.Name, s.MonthlyPrice, s.MonthlySessions,
            counts.TryGetValue(s.Id, out var c) ? c : 0)).ToList();
    }
}

// ── Detalhe do aluno ──────────────────────────────────────────
public record GetStudentDetailQuery(Guid StudentId) : IRequest<StudentDetailDto?>;

public class GetStudentDetailQueryHandler : IRequestHandler<GetStudentDetailQuery, StudentDetailDto?>
{
    private readonly IStudentRepository _students;
    private readonly ILessonRepository _lessons;

    public GetStudentDetailQueryHandler(IStudentRepository students, ILessonRepository lessons)
    {
        _students = students;
        _lessons = lessons;
    }

    public async Task<StudentDetailDto?> Handle(GetStudentDetailQuery request, CancellationToken ct)
    {
        var s = await _students.GetDetailAsync(request.StudentId, ct);
        if (s is null) return null;

        var now = DateTime.UtcNow;
        var lessons = await _lessons.GetByStudentAsync(s.Id, 30, ct);
        var done = await _lessons.CountDoneInMonthAsync(s.Id, now.Year, now.Month, ct);

        return new StudentDetailDto(
            s.Id, s.Name, s.Email, s.Instrument,
            new AssignedPlanDto(s.PlanId, s.Plan?.Name, s.MonthlyPrice, s.MonthlySessions),
            done,
            s.Repertoires.OrderBy(r => r.CreatedAt)
                .Select(r => new RepertoireDto(r.Id, r.Title, r.Composer, r.Status.ToString(), r.VideoUrl)).ToList(),
            s.Materials.OrderByDescending(m => m.CreatedAt)
                .Select(m => new MaterialDto(m.Id, m.Title, m.Type.ToString(), m.ExternalUrl)).ToList(),
            lessons.Select(l => new LessonDto(l.Id, l.StudentId, s.Name, l.ScheduledAt,
                l.DurationMinutes, l.Status.ToString(), l.Justification)).ToList());
    }
}
