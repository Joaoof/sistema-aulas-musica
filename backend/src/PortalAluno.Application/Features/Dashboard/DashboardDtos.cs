namespace PortalAluno.Application.Features.Dashboard;

public record StudentDashboardDto(
    Guid StudentId,
    string Name,
    string Instrument,
    DateTime? NextLessonAt,
    string CurrentSprint,
    IReadOnlyList<RepertoireDto> Repertoire,
    IReadOnlyList<MaterialDto> Materials,
    IReadOnlyList<BpmPointDto> BpmHistory,
    RepertoireStatsDto RepertoireStats);

public record RepertoireDto(
    Guid Id,
    string Title,
    string Composer,
    string Status,
    string? VideoUrl);

public record MaterialDto(
    Guid Id,
    string Title,
    string Type,
    string ExternalUrl);

/// <summary>Ponto da série temporal de BPM (para o AreaChart do Tremor).</summary>
public record BpmPointDto(string Date, int Bpm);

/// <summary>Proporção dominadas vs. em aprendizado (para o DonutChart do Tremor).</summary>
public record RepertoireStatsDto(int Mastered, int Learning, int Total);
