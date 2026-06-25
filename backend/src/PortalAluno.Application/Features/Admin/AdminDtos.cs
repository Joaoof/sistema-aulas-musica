using PortalAluno.Application.Features.Dashboard;

namespace PortalAluno.Application.Features.Admin;

public record PlanDto(
    Guid Id,
    string Code,
    string Name,
    int SessionsPerMonth,
    int DurationMinutes,
    decimal Price,
    string Summary,
    IReadOnlyList<string> Features);

public record StudentSummaryDto(
    Guid Id,
    string Name,
    string Email,
    string Instrument,
    string? PlanName,
    decimal? MonthlyPrice,
    int? MonthlySessions,
    int DoneThisMonth);

public record LessonDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Status,
    string? Justification);

public record AssignedPlanDto(
    Guid? PlanId,
    string? PlanName,
    decimal? MonthlyPrice,
    int? MonthlySessions);

public record StudentDetailDto(
    Guid Id,
    string Name,
    string Email,
    string Instrument,
    AssignedPlanDto Plan,
    int DoneThisMonth,
    IReadOnlyList<RepertoireDto> Repertoire,
    IReadOnlyList<MaterialDto> Materials,
    IReadOnlyList<LessonDto> Lessons);
