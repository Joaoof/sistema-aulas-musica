using MediatR;
using PortalAluno.Application.Common;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Application.Features.Practice;

public class LogPracticeCommandHandler : IRequestHandler<LogPracticeCommand, LogPracticeResult?>
{
    private readonly IStudentRepository _students;
    private readonly ICacheService _cache;

    public LogPracticeCommandHandler(IStudentRepository students, ICacheService cache)
    {
        _students = students;
        _cache = cache;
    }

    public async Task<LogPracticeResult?> Handle(LogPracticeCommand request, CancellationToken ct)
    {
        if (request.Bpm <= 0)
            throw new ArgumentException("BPM deve ser positivo.");

        var student = await _students.GetByIdAsync(request.StudentId, ct);
        if (student is null)
            return null;

        var date = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var session = student.LogPractice(date, request.Bpm);
        await _students.SaveChangesAsync(ct);

        // Invalida o dashboard cacheado -> próxima leitura reflete o novo BPM.
        await _cache.RemoveAsync(CacheKeys.StudentDashboard(request.StudentId), ct);

        return new LogPracticeResult(session.Date.ToString("dd/MM"), session.Bpm);
    }
}
