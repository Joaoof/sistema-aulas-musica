using PortalAluno.Domain.Common;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Registro de uma sessão de prática — alimenta a evolução de BPM no dashboard.
/// </summary>
public class PracticeSession : Entity
{
    public Guid StudentId { get; private set; }
    public DateOnly Date { get; private set; }
    public int Bpm { get; private set; }

    // EF Core
    private PracticeSession() { }

    public PracticeSession(Guid studentId, DateOnly date, int bpm)
    {
        if (bpm <= 0)
            throw new ArgumentException("BPM deve ser positivo.", nameof(bpm));

        StudentId = studentId;
        Date = date;
        Bpm = bpm;
    }
}
