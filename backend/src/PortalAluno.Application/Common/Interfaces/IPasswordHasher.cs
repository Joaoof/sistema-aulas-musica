namespace PortalAluno.Application.Common.Interfaces;

/// <summary>Hash e verificação de senhas (implementado com PBKDF2 na Infrastructure).</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
