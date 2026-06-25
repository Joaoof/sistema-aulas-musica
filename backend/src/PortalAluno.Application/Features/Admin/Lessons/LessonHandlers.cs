using MediatR;
using PortalAluno.Application.Common;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Application.Features.Admin.Lessons;

// ── Agendar aula ──────────────────────────────────────────────
public record ScheduleLessonCommand(Guid StudentId, DateTime ScheduledAt, int DurationMinutes)
    : IRequest<LessonDto?>;

public class ScheduleLessonCommandHandler : IRequestHandler<ScheduleLessonCommand, LessonDto?>
{
    private readonly IStudentRepository _students;
    private readonly ILessonRepository _lessons;
    private readonly ICacheService _cache;

    public ScheduleLessonCommandHandler(IStudentRepository students, ILessonRepository lessons, ICacheService cache)
    {
        _students = students;
        _lessons = lessons;
        _cache = cache;
    }

    public async Task<LessonDto?> Handle(ScheduleLessonCommand request, CancellationToken ct)
    {
        var student = await _students.GetByIdAsync(request.StudentId, ct);
        if (student is null) return null;

        var lesson = new Lesson(student.Id, request.ScheduledAt, request.DurationMinutes);
        await _lessons.AddAsync(lesson, ct);

        // Atualiza "próxima aula" do dashboard se esta for futura
        if (request.ScheduledAt > DateTime.UtcNow &&
            (student.NextLessonAt is null || request.ScheduledAt < student.NextLessonAt))
        {
            student.ScheduleNextLesson(request.ScheduledAt);
            await _cache.RemoveAsync(CacheKeys.StudentDashboard(student.Id), ct);
        }

        await _lessons.SaveChangesAsync(ct);

        return new LessonDto(lesson.Id, student.Id, student.Name, lesson.ScheduledAt,
            lesson.DurationMinutes, lesson.Status.ToString(), lesson.Justification);
    }
}

// ── Marcar feita ──────────────────────────────────────────────
public record CompleteLessonCommand(Guid LessonId) : IRequest<bool>;

public class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, bool>
{
    private readonly ILessonRepository _lessons;
    public CompleteLessonCommandHandler(ILessonRepository lessons) => _lessons = lessons;

    public async Task<bool> Handle(CompleteLessonCommand request, CancellationToken ct)
    {
        var lesson = await _lessons.GetByIdAsync(request.LessonId, ct);
        if (lesson is null) return false;
        lesson.MarkDone();
        await _lessons.SaveChangesAsync(ct);
        return true;
    }
}

// ── Justificar (não feita) ────────────────────────────────────
public record JustifyLessonCommand(Guid LessonId, string Reason) : IRequest<bool>;

public class JustifyLessonCommandHandler : IRequestHandler<JustifyLessonCommand, bool>
{
    private readonly ILessonRepository _lessons;
    public JustifyLessonCommandHandler(ILessonRepository lessons) => _lessons = lessons;

    public async Task<bool> Handle(JustifyLessonCommand request, CancellationToken ct)
    {
        var lesson = await _lessons.GetByIdAsync(request.LessonId, ct);
        if (lesson is null) return false;
        lesson.Justify(request.Reason);
        await _lessons.SaveChangesAsync(ct);
        return true;
    }
}

// ── Checklist do dia ──────────────────────────────────────────
public record GetTodayLessonsQuery : IRequest<IReadOnlyList<LessonDto>>;

public class GetTodayLessonsQueryHandler : IRequestHandler<GetTodayLessonsQuery, IReadOnlyList<LessonDto>>
{
    private readonly ILessonRepository _lessons;
    private readonly IStudentRepository _students;

    public GetTodayLessonsQueryHandler(ILessonRepository lessons, IStudentRepository students)
    {
        _lessons = lessons;
        _students = students;
    }

    public async Task<IReadOnlyList<LessonDto>> Handle(GetTodayLessonsQuery request, CancellationToken ct)
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1);
        var lessons = await _lessons.GetByRangeAsync(start, end, ct);
        var names = (await _students.GetAllWithPlanAsync(ct)).ToDictionary(s => s.Id, s => s.Name);

        return lessons.Select(l => new LessonDto(
            l.Id, l.StudentId,
            names.TryGetValue(l.StudentId, out var n) ? n : "—",
            l.ScheduledAt, l.DurationMinutes, l.Status.ToString(), l.Justification)).ToList();
    }
}

// ── Stats mensais (aulas feitas por aluno) ────────────────────
public record GetMonthlyStatsQuery(int Year, int Month) : IRequest<IReadOnlyList<StudentSummaryDto>>;

public class GetMonthlyStatsQueryHandler : IRequestHandler<GetMonthlyStatsQuery, IReadOnlyList<StudentSummaryDto>>
{
    private readonly ILessonRepository _lessons;
    private readonly IStudentRepository _students;

    public GetMonthlyStatsQueryHandler(ILessonRepository lessons, IStudentRepository students)
    {
        _lessons = lessons;
        _students = students;
    }

    public async Task<IReadOnlyList<StudentSummaryDto>> Handle(GetMonthlyStatsQuery request, CancellationToken ct)
    {
        var students = await _students.GetAllWithPlanAsync(ct);
        var counts = await _lessons.CountDoneByStudentInMonthAsync(request.Year, request.Month, ct);

        return students.Select(s => new StudentSummaryDto(
            s.Id, s.Name, s.Email, s.Instrument, s.Plan?.Name, s.MonthlyPrice, s.MonthlySessions,
            counts.TryGetValue(s.Id, out var c) ? c : 0)).ToList();
    }
}
