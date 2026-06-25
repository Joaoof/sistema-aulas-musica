using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PortalAluno.API.Controllers;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Repositories;
using Xunit;

namespace PortalAluno.Tests.Auth;

public class AuthControllerTests
{
    private readonly IStudentRepository _repo = Substitute.For<IStudentRepository>();
    private readonly IAdminRepository _admins = Substitute.For<IAdminRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();

    private AuthController CreateSut() => new(_repo, _admins, _hasher, _tokens);

    [Fact]
    public async Task Login_PorEmail_RetornaToken_E_DadosDoAluno()
    {
        var student = new Student("Ana", "ana@portal.dev", "Piano");
        _repo.GetByEmailAsync("ana@portal.dev", Arg.Any<CancellationToken>()).Returns(student);
        _tokens.CreateToken(student.Id, student.Email, Arg.Any<string>()).Returns("jwt-token");

        var action = await CreateSut().Login(new AuthController.LoginRequest("ana@portal.dev"), default);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var body = Assert.IsType<AuthController.LoginResponse>(ok.Value);
        Assert.Equal("jwt-token", body.Token);
        Assert.Equal(student.Id, body.Id);
        Assert.Equal("Piano", body.Instrument);
    }

    [Fact]
    public async Task Login_PorId_QuandoGuidValido_BuscaPorId()
    {
        var student = new Student("Ana", "ana@portal.dev", "Piano");
        _repo.GetByIdAsync(student.Id, Arg.Any<CancellationToken>()).Returns(student);
        _tokens.CreateToken(student.Id, student.Email, Arg.Any<string>()).Returns("jwt-token");

        var action = await CreateSut().Login(
            new AuthController.LoginRequest(student.Id.ToString()), default);

        Assert.IsType<OkObjectResult>(action.Result);
        await _repo.Received(1).GetByIdAsync(student.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_AlunoInexistente_RetornaNotFound()
    {
        _repo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Student?)null);

        var action = await CreateSut().Login(new AuthController.LoginRequest("nao@existe.dev"), default);

        Assert.IsType<NotFoundObjectResult>(action.Result);
    }

    [Fact]
    public async Task Login_IdentifierVazio_RetornaBadRequest()
    {
        var action = await CreateSut().Login(new AuthController.LoginRequest(""), default);
        Assert.IsType<BadRequestObjectResult>(action.Result);
    }
}
