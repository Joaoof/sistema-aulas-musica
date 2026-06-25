using Microsoft.EntityFrameworkCore;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Infrastructure.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _db;
    public AdminRepository(AppDbContext db) => _db = db;

    public Task<Admin?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Admins.FirstOrDefaultAsync(a => a.Email == email.Trim().ToLower(), ct);

    public async Task AddAsync(Admin admin, CancellationToken ct = default)
        => await _db.Admins.AddAsync(admin, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
