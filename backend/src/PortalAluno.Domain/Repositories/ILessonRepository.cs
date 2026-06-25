using PortalAluno.Domain.Entities;

namespace PortalAluno.Domain.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Lesson lesson, CancellationToken ct = default);

    /// <summary>Aulas agendadas no intervalo [start, end), todos os alunos (para o checklist do dia).</summary>
    Task<IReadOnlyList<Lesson>> GetByRangeAsync(DateTime startUtc, DateTime endUtc, CancellationToken ct = default);

    /// <summary>Aulas recentes de um aluno (detalhe admin).</summary>
    Task<IReadOnlyList<Lesson>> GetByStudentAsync(Guid studentId, int take, CancellationToken ct = default);

    /// <summary>Contagem de aulas FEITAS por aluno no mês informado.</summary>
    Task<IReadOnlyDictionary<Guid, int>> CountDoneByStudentInMonthAsync(int year, int month, CancellationToken ct = default);

    Task<int> CountDoneInMonthAsync(Guid studentId, int year, int month, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
