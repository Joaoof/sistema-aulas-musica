using NSubstitute;
using PortalAluno.Application.Common;
using PortalAluno.Application.Features.Practice;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Repositories;
using PortalAluno.Tests.Fakes;
using Xunit;

namespace PortalAluno.Tests.Practice;

public class LogPracticeCommandHandlerTests
{
    private readonly IStudentRepository _repo = Substitute.For<IStudentRepository>();
    private readonly FakeCacheService _cache = new();

    private LogPracticeCommandHandler CreateSut() => new(_repo, _cache);

    [Fact]
    public async Task Handle_RegistraSessao_E_InvalidaCacheDoDashboard()
    {
        var student = new Student("Ana", "ana@portal.dev", "Piano");
        _repo.GetByIdAsync(student.Id, Arg.Any<CancellationToken>()).Returns(student);

        var cacheKey = CacheKeys.StudentDashboard(student.Id);
        _cache.Seed(cacheKey, "dashboard-antigo"); // simula dashboard já cacheado

        var result = await CreateSut().Handle(new LogPracticeCommand(student.Id, 120), default);

        Assert.NotNull(result);
        Assert.Equal(120, result!.Bpm);
        Assert.Single(student.PracticeSessions);
        Assert.Equal(120, student.PracticeSessions.First().Bpm);
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.False(_cache.Contains(cacheKey)); // cache foi invalidado
    }

    [Fact]
    public async Task Handle_AlunoInexistente_RetornaNull_E_NaoSalva()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Student?)null);

        var result = await CreateSut().Handle(new LogPracticeCommand(Guid.NewGuid(), 100), default);

        Assert.Null(result);
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public async Task Handle_BpmInvalido_LancaArgumentException(int bpm)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            CreateSut().Handle(new LogPracticeCommand(Guid.NewGuid(), bpm), default));
    }

    [Fact]
    public async Task Handle_SemData_UsaDataDeHojeUtc()
    {
        var student = new Student("Ana", "ana@portal.dev", "Piano");
        _repo.GetByIdAsync(student.Id, Arg.Any<CancellationToken>()).Returns(student);

        await CreateSut().Handle(new LogPracticeCommand(student.Id, 90), default);

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        Assert.Equal(hoje, student.PracticeSessions.First().Date);
    }
}
