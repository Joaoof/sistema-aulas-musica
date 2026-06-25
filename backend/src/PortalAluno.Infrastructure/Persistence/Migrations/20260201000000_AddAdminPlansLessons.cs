using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PortalAluno.Infrastructure.Persistence;

#nullable disable

namespace PortalAluno.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260201000000_AddAdminPlansLessons")]
/// <inheritdoc />
public partial class AddAdminPlansLessons : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ── plans ──────────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "plans",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                SessionsPerMonth = table.Column<int>(type: "integer", nullable: false),
                DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                Summary = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Features = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_plans", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_plans_Code", table: "plans", column: "Code", unique: true);

        // ── admins ─────────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "admins",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_admins", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_admins_Email", table: "admins", column: "Email", unique: true);

        // ── students: colunas de plano ─────────────────────────
        migrationBuilder.AddColumn<Guid>(
            name: "PlanId", table: "students", type: "uuid", nullable: true);
        migrationBuilder.AddColumn<decimal>(
            name: "MonthlyPrice", table: "students", type: "numeric(10,2)", nullable: true);
        migrationBuilder.AddColumn<int>(
            name: "MonthlySessions", table: "students", type: "integer", nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_students_PlanId", table: "students", column: "PlanId");
        migrationBuilder.AddForeignKey(
            name: "FK_students_plans_PlanId",
            table: "students", column: "PlanId",
            principalTable: "plans", principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        // ── lessons ────────────────────────────────────────────
        migrationBuilder.CreateTable(
            name: "lessons",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Justification = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lessons", x => x.Id);
                table.ForeignKey(
                    name: "FK_lessons_students_StudentId",
                    column: x => x.StudentId,
                    principalTable: "students", principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_lessons_ScheduledAt", table: "lessons", column: "ScheduledAt");
        migrationBuilder.CreateIndex(
            name: "IX_lessons_StudentId_Status", table: "lessons", columns: new[] { "StudentId", "Status" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "lessons");
        migrationBuilder.DropTable(name: "admins");
        migrationBuilder.DropForeignKey(name: "FK_students_plans_PlanId", table: "students");
        migrationBuilder.DropIndex(name: "IX_students_PlanId", table: "students");
        migrationBuilder.DropColumn(name: "PlanId", table: "students");
        migrationBuilder.DropColumn(name: "MonthlyPrice", table: "students");
        migrationBuilder.DropColumn(name: "MonthlySessions", table: "students");
        migrationBuilder.DropTable(name: "plans");
    }
}
