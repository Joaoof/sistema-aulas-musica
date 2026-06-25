namespace PortalAluno.Domain.Common;

/// <summary>
/// Raiz base para entidades do domínio. Identidade por Id (Guid).
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public override bool Equals(object? obj)
        => obj is Entity other && other.GetType() == GetType() && other.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();
}
