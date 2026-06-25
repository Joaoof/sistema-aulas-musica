using Microsoft.EntityFrameworkCore;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Infrastructure.Persistence.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _db;
    public PlanRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Plan>> GetAllAsync(CancellationToken ct = default)
        => await _db.Plans.AsNoTracking().OrderBy(p => p.DisplayOrder).ToListAsync(ct);

    public Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Plans.FirstOrDefaultAsync(p => p.Id == id, ct);
}
