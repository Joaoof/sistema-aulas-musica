using MediatR;
using PortalAluno.Application.Common;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Enums;
using PortalAluno.Domain.Repositories;

namespace PortalAluno.Application.Features.Admin.Students;

// ── Criar aluno ───────────────────────────────────────────────
public record CreateStudentCommand(string Name, string Email, string Instrument)
    : IRequest<StudentSummaryDto>;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentSummaryDto>
{
    private readonly IStudentRepository _students;
    public CreateStudentCommandHandler(IStudentRepository students) => _students = students;

    public async Task<StudentSummaryDto> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var student = new Student(request.Name, request.Email, request.Instrument);
        await _students.AddAsync(student, ct);
        await _students.SaveChangesAsync(ct);
        return new StudentSummaryDto(student.Id, student.Name, student.Email, student.Instrument,
            null, null, null, 0);
    }
}

// ── Excluir aluno ─────────────────────────────────────────────
public record DeleteStudentCommand(Guid StudentId) : IRequest<bool>;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, bool>
{
    private readonly IStudentRepository _students;
    private readonly ICacheService _cache;

    public DeleteStudentCommandHandler(IStudentRepository students, ICacheService cache)
    {
        _students = students;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken ct)
    {
        var student = await _students.GetByIdAsync(request.StudentId, ct);
        if (student is null) return false;

        // Aulas, repertório, materiais e práticas saem por ON DELETE CASCADE.
        _students.Remove(student);
        await _students.SaveChangesAsync(ct);
        await _cache.RemoveAsync(CacheKeys.StudentDashboard(request.StudentId), ct);
        return true;
    }
}

// ── Atribuir/editar plano ─────────────────────────────────────
public record AssignPlanCommand(Guid StudentId, Guid PlanId, decimal? MonthlyPrice, int? MonthlySessions)
    : IRequest<AssignedPlanDto?>;

public class AssignPlanCommandHandler : IRequestHandler<AssignPlanCommand, AssignedPlanDto?>
{
    private readonly IStudentRepository _students;
    private readonly IPlanRepository _plans;
    private readonly ICacheService _cache;

    public AssignPlanCommandHandler(IStudentRepository students, IPlanRepository plans, ICacheService cache)
    {
        _students = students;
        _plans = plans;
        _cache = cache;
    }

    public async Task<AssignedPlanDto?> Handle(AssignPlanCommand request, CancellationToken ct)
    {
        var student = await _students.GetByIdAsync(request.StudentId, ct);
        if (student is null) return null;

        var plan = await _plans.GetByIdAsync(request.PlanId, ct);
        if (plan is null) return null;

        student.AssignPlan(plan, request.MonthlyPrice, request.MonthlySessions);
        await _students.SaveChangesAsync(ct);
        await _cache.RemoveAsync(CacheKeys.StudentDashboard(student.Id), ct);

        return new AssignedPlanDto(plan.Id, plan.Name, student.MonthlyPrice, student.MonthlySessions);
    }
}

// ── Adicionar repertório ──────────────────────────────────────
public record AddRepertoireCommand(Guid StudentId, string Title, string Composer, string? VideoUrl, string? Status)
    : IRequest<bool>;

public class AddRepertoireCommandHandler : IRequestHandler<AddRepertoireCommand, bool>
{
    private readonly IStudentRepository _students;
    private readonly ICacheService _cache;

    public AddRepertoireCommandHandler(IStudentRepository students, ICacheService cache)
    {
        _students = students;
        _cache = cache;
    }

    public async Task<bool> Handle(AddRepertoireCommand request, CancellationToken ct)
    {
        var student = await _students.GetByIdAsync(request.StudentId, ct);
        if (student is null) return false;

        var rep = student.AddRepertoire(request.Title, request.Composer, request.VideoUrl);
        if (Enum.TryParse<RepertoireStatus>(request.Status, out var status))
            rep.Advance(status);

        await _students.SaveChangesAsync(ct);
        await _cache.RemoveAsync(CacheKeys.StudentDashboard(student.Id), ct);
        return true;
    }
}

// ── Adicionar material (URL externa do Drive) ─────────────────
public record AddMaterialCommand(Guid StudentId, string Title, string Type, string ExternalUrl)
    : IRequest<bool>;

public class AddMaterialCommandHandler : IRequestHandler<AddMaterialCommand, bool>
{
    private readonly IStudentRepository _students;
    private readonly ICacheService _cache;

    public AddMaterialCommandHandler(IStudentRepository students, ICacheService cache)
    {
        _students = students;
        _cache = cache;
    }

    public async Task<bool> Handle(AddMaterialCommand request, CancellationToken ct)
    {
        var student = await _students.GetByIdAsync(request.StudentId, ct);
        if (student is null) return false;

        if (!Enum.TryParse<MaterialType>(request.Type, out var type))
            throw new ArgumentException($"Tipo de material inválido: {request.Type}");

        student.AddMaterial(request.Title, type, request.ExternalUrl);
        await _students.SaveChangesAsync(ct);
        await _cache.RemoveAsync(CacheKeys.StudentDashboard(student.Id), ct);
        return true;
    }
}
