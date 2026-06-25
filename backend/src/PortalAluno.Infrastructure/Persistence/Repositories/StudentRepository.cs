using Microsoft.EntityFrameworkCore;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Infrastructure.Persistence.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;

    public StudentRepository(AppDbContext db) => _db = db;

    public Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Students.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Students.FirstOrDefaultAsync(s => s.Email == email.Trim().ToLower(), ct);

    public async Task<IReadOnlyList<Student>> GetAllWithPlanAsync(CancellationToken ct = default)
        => await _db.Students.AsNoTracking()
            .Include(s => s.Plan)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public Task<Student?> GetDetailAsync(Guid id, CancellationToken ct = default)
        => _db.Students.AsNoTracking()
            .Include(s => s.Plan)
            .Include(s => s.Repertoires)
            .Include(s => s.Materials)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Student?> GetDashboardAggregateAsync(Guid id, CancellationToken ct = default)
        => _db.Students
            .AsNoTracking()
            .Include(s => s.Repertoires)
            .Include(s => s.Materials)
            .Include(s => s.PracticeSessions)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task AddAsync(Student student, CancellationToken ct = default)
        => await _db.Students.AddAsync(student, ct);

    public void Remove(Student student) => _db.Students.Remove(student);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
