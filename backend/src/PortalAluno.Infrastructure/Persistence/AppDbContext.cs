using Microsoft.EntityFrameworkCore;
using PortalAluno.Domain.Entities;

namespace PortalAluno.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Repertoire> Repertoires => Set<Repertoire>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
