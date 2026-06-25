using Microsoft.EntityFrameworkCore;
using PortalAluno.Application.Common.Interfaces;
using PortalAluno.Domain.Entities;
using PortalAluno.Domain.Enums;

namespace PortalAluno.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHasher hasher,
        (string Name, string Email, string Password) admin,
        CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        await SeedPlansAsync(db, ct);
        await SeedAdminAsync(db, hasher, admin, ct);
        await SeedDemoStudentAsync(db, ct);
    }

    private static async Task SeedPlansAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Plans.AnyAsync(ct)) return;

        db.Plans.AddRange(
            new Plan("start", "Start & Suporte", 2, 40, 180m,
                "Quinzenal. Professor em casa a cada 15 dias.",
                "Acesso a Vídeos de Treino\nSuporte via WhatsApp nos dias sem aula", 1),
            new Plan("pratico", "Prático Regular", 4, 40, 280m,
                "1x por semana. O formato padrão para constância.",
                "Tudo do Start\nRepertório 100% Personalizado\nMétodo de Sprints: música dividida em fases semanais", 2),
            new Plan("evolution", "Evolution Premium", 4, 60, 380m,
                "1x por semana (60 min). Maior rendimento e organização tecnológica.",
                "Tudo dos anteriores\nPortal Exclusivo do Aluno (histórico, PDFs, cronogramas)\nMonitoramento Ativo: lembretes de treino", 3),
            new Plan("imersao", "Imersão / Fast Track", 12, 60, 900m,
                "3x na semana. Urgência absoluta ou acompanhamento rigoroso.",
                "Simulações práticas\nPrioridade de agenda", 4));

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedAdminAsync(
        AppDbContext db, IPasswordHasher hasher,
        (string Name, string Email, string Password) admin, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(admin.Email) || string.IsNullOrWhiteSpace(admin.Password))
            return;

        var exists = await db.Admins.AnyAsync(a => a.Email == admin.Email.ToLower(), ct);
        if (exists) return;

        db.Admins.Add(new Admin(admin.Name, admin.Email, hasher.Hash(admin.Password)));
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedDemoStudentAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Students.AnyAsync(ct)) return;

        var aluno = new Student("Ana Beatriz", "ana@portal.dev", "Piano",
            nextLessonAt: new DateTime(2026, 06, 27, 14, 0, 0, DateTimeKind.Utc));

        var emTreino = aluno.AddRepertoire("Clair de Lune", "Claude Debussy",
            "https://drive.google.com/file/d/EXEMPLO_VIDEO_1/view");
        emTreino.Advance(RepertoireStatus.InProgress);

        var dominada = aluno.AddRepertoire("Prelúdio em C Maior", "J. S. Bach",
            "https://drive.google.com/file/d/EXEMPLO_VIDEO_2/view");
        dominada.Advance(RepertoireStatus.Mastered);

        aluno.AddRepertoire("Gymnopédie No.1", "Erik Satie",
            "https://drive.google.com/file/d/EXEMPLO_VIDEO_4/view");

        aluno.AddMaterial("Partitura - Clair de Lune", MaterialType.Sheet,
            "https://drive.google.com/file/d/EXEMPLO_PDF_1/view");
        aluno.AddMaterial("Exercícios de Hanon (PDF)", MaterialType.Pdf,
            "https://drive.google.com/file/d/EXEMPLO_PDF_2/view");
        aluno.AddMaterial("Aula gravada - Postura e técnica", MaterialType.Video,
            "https://drive.google.com/file/d/EXEMPLO_VIDEO_3/view");

        var baseDate = new DateOnly(2026, 05, 01);
        int[] bpms = { 60, 66, 72, 70, 80, 88, 92, 100 };
        for (var i = 0; i < bpms.Length; i++)
            aluno.LogPractice(baseDate.AddDays(i * 5), bpms[i]);

        // Atribui o plano Evolution (com valores herdados do catálogo)
        var evolution = await db.Plans.FirstAsync(p => p.Code == "evolution", ct);
        aluno.AssignPlan(evolution);

        db.Students.Add(aluno);
        await db.SaveChangesAsync(ct);

        // Aulas: 2 já feitas neste mês + 1 agendada para hoje (aparece no checklist)
        var today = DateTime.UtcNow.Date;
        var firstOfMonth = new DateTime(today.Year, today.Month, 1, 10, 0, 0, DateTimeKind.Utc);

        var feita1 = new Lesson(aluno.Id, firstOfMonth, 60); feita1.MarkDone();
        var feita2 = new Lesson(aluno.Id, firstOfMonth.AddDays(7), 60); feita2.MarkDone();
        var hoje = new Lesson(aluno.Id, new DateTime(today.Year, today.Month, today.Day, 14, 0, 0, DateTimeKind.Utc), 60);

        db.Lessons.AddRange(feita1, feita2, hoje);
        await db.SaveChangesAsync(ct);
    }
}
