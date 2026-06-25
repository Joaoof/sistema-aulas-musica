using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PortalAluno.Infrastructure.Persistence;

/// <summary>
/// Factory usada apenas em design-time pelo `dotnet ef` (migrations).
/// Em runtime o DbContext vem do contêiner de DI.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=postgres;Port=5432;Database=portal_aluno;Username=portal;Password=portal_dev_pwd";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connection, npg =>
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options;

        return new AppDbContext(options);
    }
}
