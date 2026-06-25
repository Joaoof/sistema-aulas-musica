using PortalAluno.Domain.Common;
using PortalAluno.Domain.Enums;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Peça/repertório que o aluno está estudando.
/// O vídeo de referência é uma URL externa (Google Drive).
/// </summary>
public class Repertoire : Entity
{
    public Guid StudentId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Composer { get; private set; } = default!;
    public RepertoireStatus Status { get; private set; } = RepertoireStatus.ToStudy;
    public string? VideoUrl { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // EF Core
    private Repertoire() { }

    public Repertoire(Guid studentId, string title, string composer, string? videoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título é obrigatório.", nameof(title));

        StudentId = studentId;
        Title = title.Trim();
        Composer = composer.Trim();
        VideoUrl = videoUrl;
    }

    public void Advance(RepertoireStatus status) => Status = status;
}
