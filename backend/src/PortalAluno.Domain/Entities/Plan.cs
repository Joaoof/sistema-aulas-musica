using PortalAluno.Domain.Common;

namespace PortalAluno.Domain.Entities;

/// <summary>
/// Pacote/plano mensal do catálogo (Start, Prático, Evolution, Imersão).
/// Os valores podem ser sobrescritos por aluno na atribuição.
/// </summary>
public class Plan : Entity
{
    public string Code { get; private set; } = default!;   // start | pratico | evolution | imersao
    public string Name { get; private set; } = default!;
    public int SessionsPerMonth { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public string Summary { get; private set; } = default!;
    public string Features { get; private set; } = default!; // bullets separados por '\n'
    public int DisplayOrder { get; private set; }

    private Plan() { }

    public Plan(string code, string name, int sessionsPerMonth, int durationMinutes,
        decimal price, string summary, string features, int displayOrder)
    {
        Code = code;
        Name = name;
        SessionsPerMonth = sessionsPerMonth;
        DurationMinutes = durationMinutes;
        Price = price;
        Summary = summary;
        Features = features;
        DisplayOrder = displayOrder;
    }
}
