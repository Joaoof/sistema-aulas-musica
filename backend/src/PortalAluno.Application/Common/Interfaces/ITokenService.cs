namespace PortalAluno.Application.Common.Interfaces;

/// <summary>
/// Emissão de tokens de acesso (JWT). Implementado na borda (API/Infra).
/// </summary>
public interface ITokenService
{
    string CreateToken(Guid userId, string email, string role);
}
