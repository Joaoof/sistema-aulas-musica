using MediatR;

namespace PortalAluno.Application.Features.Practice;

/// <summary>
/// Registra uma sessão de prática (BPM) para o aluno.
/// Se <paramref name="Date"/> for nulo, usa a data atual (UTC).
/// </summary>
public record LogPracticeCommand(Guid StudentId, int Bpm, DateOnly? Date = null)
    : IRequest<LogPracticeResult?>;

public record LogPracticeResult(string Date, int Bpm);
