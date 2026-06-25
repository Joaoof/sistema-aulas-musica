using PortalAluno.Domain.Entities;

namespace PortalAluno.Domain.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Todos os alunos com o plano carregado (para a área admin).</summary>
    Task<IReadOnlyList<Student>> GetAllWithPlanAsync(CancellationToken ct = default);

    /// <summary>Aluno com plano, repertório e materiais (detalhe admin).</summary>
    Task<Student?> GetDetailAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Carrega o aluno com repertório e materiais (para montar o dashboard).
    /// </summary>
    Task<Student?> GetDashboardAggregateAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(Student student, CancellationToken ct = default);

    void Remove(Student student);

    Task SaveChangesAsync(CancellationToken ct = default);
}
