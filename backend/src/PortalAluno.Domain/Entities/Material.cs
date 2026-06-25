using PortalAluno.Domain.Common;
using PortalAluno.Domain.Enums;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Material de apoio (PDF, vídeo, áudio, partitura).
/// Estratégia de arquivos: SEMPRE referencia uma URL externa do Google Drive.
/// </summary>
public class Material : Entity
{
    public Guid StudentId { get; private set; }
    public string Title { get; private set; } = default!;
    public MaterialType Type { get; private set; }
    public string ExternalUrl { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // EF Core
    private Material() { }

    public Material(Guid studentId, string title, MaterialType type, string externalUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório.", nameof(title));
        if (string.IsNullOrWhiteSpace(externalUrl))
            throw new ArgumentException("URL externa é obrigatória.", nameof(externalUrl));

        StudentId = studentId;
        Title = title.Trim();
        Type = type;
        ExternalUrl = externalUrl.Trim();
    }
}
