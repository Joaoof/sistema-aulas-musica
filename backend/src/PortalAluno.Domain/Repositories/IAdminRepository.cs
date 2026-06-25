using PortalAluno.Domain.Entities;

namespace PortalAluno.Domain.Repositories;

public interface IAdminRepository
{
    Task<Admin?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Admin admin, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
