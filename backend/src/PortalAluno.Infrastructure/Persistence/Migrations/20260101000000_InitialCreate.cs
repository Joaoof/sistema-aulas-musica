using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PortalAluno.Infrastructure.Persistence;

#nullable disable

namespace PortalAluno.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260101000000_InitialCreate")]
/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "students",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Instrument = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                NextLessonAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_students", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "repertoires",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Composer = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                VideoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_repertoires", x => x.Id);
                table.ForeignKey(
                    name: "FK_repertoires_students_StudentId",
                    column: x => x.StudentId,
                    principalTable: "students",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "materials",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                ExternalUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_materials", x => x.Id);
                table.ForeignKey(
                    name: "FK_materials_students_StudentId",
                    column: x => x.StudentId,
                    principalTable: "students",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "practice_sessions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                Bpm = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_practice_sessions", x => x.Id);
                table.ForeignKey(
                    name: "FK_practice_sessions_students_StudentId",
                    column: x => x.StudentId,
                    principalTable: "students",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_students_Email",
            table: "students",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_practice_sessions_StudentId_Date",
            table: "practice_sessions",
            columns: new[] { "StudentId", "Date" });

        migrationBuilder.CreateIndex(
            name: "IX_repertoires_StudentId",
            table: "repertoires",
            column: "StudentId");

        migrationBuilder.CreateIndex(
            name: "IX_materials_StudentId",
            table: "materials",
            column: "StudentId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "repertoires");
        migrationBuilder.DropTable(name: "materials");
        migrationBuilder.DropTable(name: "practice_sessions");
        migrationBuilder.DropTable(name: "students");
    }
}
