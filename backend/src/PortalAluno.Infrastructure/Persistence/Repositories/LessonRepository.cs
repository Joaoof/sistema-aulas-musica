using Microsoft.EntityFrameworkCore;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Enums;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Infrastructure.Persistence.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _db;
    public LessonRepository(AppDbContext db) => _db = db;

    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Lessons.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task AddAsync(Lesson lesson, CancellationToken ct = default)
        => await _db.Lessons.AddAsync(lesson, ct);

    public async Task<IReadOnlyList<Lesson>> GetByRangeAsync(
        DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
        => await _db.Lessons.AsNoTracking()
            .Where(l => l.ScheduledAt >= startUtc && l.ScheduledAt < endUtc)
            .OrderBy(l => l.ScheduledAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Lesson>> GetByStudentAsync(
        Guid studentId, int take, CancellationToken ct = default)
        => await _db.Lessons.AsNoTracking()
            .Where(l => l.StudentId == studentId)
            .OrderByDescending(l => l.ScheduledAt)
            .Take(take)
            .ToListAsync(ct);

    public async Task<IReadOnlyDictionary<Guid, int>> CountDoneByStudentInMonthAsync(
        int year, int month, CancellationToken ct = default)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return await _db.Lessons.AsNoTracking()
            .Where(l => l.Status == LessonStatus.Done
                        && l.ScheduledAt >= start && l.ScheduledAt < end)
            .GroupBy(l => l.StudentId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
    }

    public Task<int> CountDoneInMonthAsync(Guid studentId, int year, int month, CancellationToken ct = default)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return _db.Lessons.AsNoTracking()
            .CountAsync(l => l.StudentId == studentId && l.Status == LessonStatus.Done
                             && l.ScheduledAt >= start && l.ScheduledAt < end, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
