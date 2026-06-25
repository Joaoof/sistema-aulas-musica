using Microsoft.AspNetCore.Mvc;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IStudentRepository _students;
    private readonly IAdminRepository _admins;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthController(
        IStudentRepository students, IAdminRepository admins,
        IPasswordHasher hasher, ITokenService tokens)
    {
        _students = students;
        _admins = admins;
        _hasher = hasher;
        _tokens = tokens;
    }

    public record LoginRequest(string Identifier);
    public record LoginResponse(string Token, Guid Id, string Name, string Email, string Instrument, string Role);

    public record AdminLoginRequest(string Email, string Password);
    public record AdminLoginResponse(string Token, Guid Id, string Name, string Email, string Role);

    /// <summary>Login do aluno (e-mail OU Id) — emite JWT com role Student.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier))
            return BadRequest(new { message = "Informe e-mail ou Id." });

        var student = Guid.TryParse(request.Identifier, out var id)
            ? await _students.GetByIdAsync(id, ct)
            : await _students.GetByEmailAsync(request.Identifier, ct);

        if (student is null)
            return NotFound(new { message = "Aluno não encontrado." });

        var token = _tokens.CreateToken(student.Id, student.Email, Roles.Student);
        return Ok(new LoginResponse(token, student.Id, student.Name, student.Email, student.Instrument, Roles.Student));
    }

    /// <summary>Login do super usuário (professor) — e-mail + senha, emite JWT com role Admin.</summary>
    [HttpPost("admin/login")]
    public async Task<ActionResult<AdminLoginResponse>> AdminLogin(
        [FromBody] AdminLoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Informe e-mail e senha." });

        var admin = await _admins.GetByEmailAsync(request.Email, ct);
        if (admin is null || !_hasher.Verify(request.Password, admin.PasswordHash))
            return Unauthorized(new { message = "Credenciais inválidas." });

        var token = _tokens.CreateToken(admin.Id, admin.Email, Roles.Admin);
        return Ok(new AdminLoginResponse(token, admin.Id, admin.Name, admin.Email, Roles.Admin));
    }
}
