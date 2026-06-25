using PortalAluno.Domain.Common;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Aluno — raiz de agregação. Possui repertório e materiais associados.
/// </summary>
public class Student : Entity
{
    private readonly List<Repertoire> _repertoires = new();
    private readonly List<Material> _materials = new();
    private readonly List<PracticeSession> _practiceSessions = new();

    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Instrument { get; private set; } = default!;
    public DateTime? NextLessonAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Plano atribuído (catálogo) + valores editáveis por aluno
    public Guid? PlanId { get; private set; }
    public Plan? Plan { get; private set; }
    public decimal? MonthlyPrice { get; private set; }
    public int? MonthlySessions { get; private set; }

    public IReadOnlyCollection<Repertoire> Repertoires => _repertoires.AsReadOnly();
    public IReadOnlyCollection<Material> Materials => _materials.AsReadOnly();
    public IReadOnlyCollection<PracticeSession> PracticeSessions => _practiceSessions.AsReadOnly();

    // EF Core
    private Student() { }

    public Student(string name, string email, string instrument, DateTime? nextLessonAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório.", nameof(email));

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Instrument = instrument.Trim();
        NextLessonAt = nextLessonAt;
    }

    public void ScheduleNextLesson(DateTime when) => NextLessonAt = when;

    public Repertoire AddRepertoire(string title, string composer, string? videoUrl = null)
    {
        var item = new Repertoire(Id, title, composer, videoUrl);
        _repertoires.Add(item);
        return item;
    }

    public Material AddMaterial(string title, Domain.Enums.MaterialType type, string externalUrl)
    {
        var item = new Material(Id, title, type, externalUrl);
        _materials.Add(item);
        return item;
    }

    public PracticeSession LogPractice(DateOnly date, int bpm)
    {
        var item = new PracticeSession(Id, date, bpm);
        _practiceSessions.Add(item);
        return item;
    }

    /// <summary>Atribui um plano do catálogo, herdando preço/sessões (editáveis depois).</summary>
    public void AssignPlan(Plan plan, decimal? customPrice = null, int? customSessions = null)
    {
        PlanId = plan.Id;
        Plan = plan;
        MonthlyPrice = customPrice ?? plan.Price;
        MonthlySessions = customSessions ?? plan.SessionsPerMonth;
    }

    /// <summary>Edita os valores do plano só deste aluno (override).</summary>
    public void EditPlanValues(decimal monthlyPrice, int monthlySessions)
    {
        MonthlyPrice = monthlyPrice;
        MonthlySessions = monthlySessions;
    }
}
