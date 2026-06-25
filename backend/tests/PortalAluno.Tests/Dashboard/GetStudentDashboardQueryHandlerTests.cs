using NSubstitute;
using PortalAluno.Application.Common;
using PortalAluno.Application.Features.Dashboard;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Enums;
using PortalAluno.Domain.Repositories;
using PortalAluno.Tests.Fakes;
using Xunit;

namespace PortalAluno.Tests.Dashboard;

public class GetStudentDashboardQueryHandlerTests
{
    private readonly IStudentRepository _repo = Substitute.For<IStudentRepository>();
    private readonly FakeCacheService _cache = new();

    private GetStudentDashboardQueryHandler CreateSut() => new(_repo, _cache);

    private static Student BuildStudent()
    {
        var s = new Student("Ana", "ana@portal.dev", "Piano");
        s.AddRepertoire("Clair de Lune", "Debussy").Advance(RepertoireStatus.InProgress);
        s.AddRepertoire("Prelúdio", "Bach").Advance(RepertoireStatus.Mastered);
        s.AddRepertoire("Gymnopédie", "Satie"); // ToStudy
        s.LogPractice(new DateOnly(2026, 5, 1), 60);
        s.LogPractice(new DateOnly(2026, 5, 6), 80);
        return s;
    }

    [Fact]
    public async Task Handle_CacheMiss_ConsultaRepositorio_MapeiaEStatsCorretos_E_Cacheia()
    {
        var student = BuildStudent();
        _repo.GetDashboardAggregateAsync(student.Id, Arg.Any<CancellationToken>()).Returns(student);

        var dto = await CreateSut().Handle(new GetStudentDashboardQuery(student.Id), default);

        Assert.NotNull(dto);
        Assert.Equal(3, dto!.RepertoireStats.Total);
        Assert.Equal(1, dto.RepertoireStats.Mastered);
        Assert.Equal(2, dto.RepertoireStats.Learning);
        Assert.Equal(2, dto.BpmHistory.Count);
        Assert.Contains("Clair de Lune", dto.CurrentSprint); // peça InProgress
        Assert.Equal(1, _cache.FactoryCalls);
        Assert.True(_cache.Contains(CacheKeys.StudentDashboard(student.Id)));
    }

    [Fact]
    public async Task Handle_CacheHit_NaoConsultaRepositorio()
    {
        var student = BuildStudent();
        var cached = new StudentDashboardDto(
            student.Id, "Ana", "Piano", null, "cache",
            Array.Empty<RepertoireDto>(), Array.Empty<MaterialDto>(),
            Array.Empty<BpmPointDto>(), new RepertoireStatsDto(0, 0, 0));
        _cache.Seed(CacheKeys.StudentDashboard(student.Id), cached);

        var dto = await CreateSut().Handle(new GetStudentDashboardQuery(student.Id), default);

        Assert.Same(cached, dto);
        Assert.Equal(0, _cache.FactoryCalls);
        await _repo.DidNotReceive().GetDashboardAggregateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlunoInexistente_RetornaNull_E_NaoCacheia()
    {
        _repo.GetDashboardAggregateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Student?)null);
        var id = Guid.NewGuid();

        var dto = await CreateSut().Handle(new GetStudentDashboardQuery(id), default);

        Assert.Null(dto);
        Assert.False(_cache.Contains(CacheKeys.StudentDashboard(id)));
    }
}
