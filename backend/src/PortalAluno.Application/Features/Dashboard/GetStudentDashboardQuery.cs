using MediatR;

namespace PortalAluno.Application.Features.Dashboard;

/// <summary>
/// Caso de uso: obter o dashboard consolidado de um aluno.
/// O resultado é servido a partir do cache (Redis) quando disponível.
/// </summary>
public record GetStudentDashboardQuery(Guid StudentId) : IRequest<StudentDashboardDto?>;
