using PortalAluno.Domain.Entities;

namespace PortalAluno.Domain.Repositories;

public interface IPlanRepository
{
    Task<IReadOnlyList<Plan>> GetAllAsync(CancellationToken ct = default);
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
