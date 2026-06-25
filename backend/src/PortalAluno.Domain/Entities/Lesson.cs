using PortalAluno.Domain.Common;
using PortalAluno.Domain.Enums;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Aula agendada para um aluno. O professor marca como feita ou justifica a ausência.
/// </summary>
public class Lesson : Entity
{
    public Guid StudentId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public LessonStatus Status { get; private set; } = LessonStatus.Scheduled;
    public string? Justification { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Lesson() { }

    public Lesson(Guid studentId, DateTime scheduledAt, int durationMinutes)
    {
        if (durationMinutes <= 0)
            throw new ArgumentException("Duração inválida.", nameof(durationMinutes));

        StudentId = studentId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
    }

    public void MarkDone()
    {
        Status = LessonStatus.Done;
        Justification = null;
        ResolvedAt = DateTime.UtcNow;
    }

    public void Justify(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Justificativa é obrigatória.", nameof(reason));

        Status = LessonStatus.Justified;
        Justification = reason.Trim();
        ResolvedAt = DateTime.UtcNow;
    }
}
