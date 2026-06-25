using PortalAluno.Domain.Common;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Super usuário (professor). Login por email + senha (hash).
/// </summary>
public class Admin : Entity
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;

    private Admin() { }

    public Admin(string name, string email, string passwordHash)
    {
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }

    public void SetPasswordHash(string hash) => PasswordHash = hash;
}
