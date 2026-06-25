using MediatR;
using PortalAluno.Application.Common;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Application.Features.Dashboard;

public class GetStudentDashboardQueryHandler
    : IRequestHandler<GetStudentDashboardQuery, StudentDashboardDto?>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    private readonly IStudentRepository _students;
    private readonly ICacheService _cache;

    public GetStudentDashboardQueryHandler(IStudentRepository students, ICacheService cache)
    {
        _students = students;
        _cache = cache;
    }

    public async Task<StudentDashboardDto?> Handle(
        GetStudentDashboardQuery request, CancellationToken ct)
    {
        var cacheKey = CacheKeys.StudentDashboard(request.StudentId);

        // Cache-aside: tenta o Redis, senão consulta o Postgres e cacheia.
        return await _cache.GetOrSetAsync(
            cacheKey,
            async token =>
            {
                var student = await _students.GetDashboardAggregateAsync(request.StudentId, token);
                return student is null ? null : Map(student);
            },
            CacheTtl,
            ct);
    }

    private static StudentDashboardDto Map(Domain.Entities.Student s)
    {
        var mastered = s.Repertoires.Count(r => r.Status == Domain.Enums.RepertoireStatus.Mastered);
        var total = s.Repertoires.Count;

        return new StudentDashboardDto(
            s.Id,
            s.Name,
            s.Instrument,
            s.NextLessonAt,
            CurrentSprint: BuildSprintLabel(s),
            Repertoire: s.Repertoires
                .OrderBy(r => r.CreatedAt)
                .Select(r => new RepertoireDto(r.Id, r.Title, r.Composer, r.Status.ToString(), r.VideoUrl))
                .ToList(),
            Materials: s.Materials
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MaterialDto(m.Id, m.Title, m.Type.ToString(), m.ExternalUrl))
                .ToList(),
            BpmHistory: s.PracticeSessions
                .OrderBy(p => p.Date)
                .Select(p => new BpmPointDto(p.Date.ToString("dd/MM"), p.Bpm))
                .ToList(),
            RepertoireStats: new RepertoireStatsDto(mastered, total - mastered, total));
    }

    private static string BuildSprintLabel(Domain.Entities.Student s)
    {
        var inProgress = s.Repertoires
            .FirstOrDefault(r => r.Status == Domain.Enums.RepertoireStatus.InProgress);
        return inProgress is not null
            ? $"{inProgress.Title} — {inProgress.Composer}"
            : "Sem peça em treino";
    }
}
