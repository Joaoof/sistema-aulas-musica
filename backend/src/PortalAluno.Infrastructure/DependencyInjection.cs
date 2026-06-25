using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Repositories;
using PortalAluno.Infrastructure.Cache;
using PortalAluno.Infrastructure.Persistence;
using PortalAluno.Infrastructure.Persistence.Repositories;
using PortalAluno.Infrastructure.Security;

namespace PortalAluno.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // ── PostgreSQL (EF Core) ──────────────────────────────
        var postgres = config.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres não configurada.");

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(postgres, npg =>
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── Redis (cache distribuído) ─────────────────────────
        var redis = config.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("ConnectionStrings:Redis não configurada.");

        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = redis;
            opt.InstanceName = "portal:";
        });

        // ── Repositórios + serviços ───────────────────────────
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        return services;
    }
}
