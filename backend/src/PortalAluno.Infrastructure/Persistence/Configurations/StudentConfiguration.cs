using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalAluno.Domain.Entities;

namespace PortalAluno.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> b)
    {
        b.ToTable("students");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever(); // Id gerado no domínio

        b.Property(x => x.Name).HasMaxLength(160).IsRequired();
        b.Property(x => x.Email).HasMaxLength(160).IsRequired();
        b.Property(x => x.Instrument).HasMaxLength(80).IsRequired();
        b.Property(x => x.NextLessonAt);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.MonthlyPrice).HasColumnType("numeric(10,2)");
        b.Property(x => x.MonthlySessions);

        b.HasIndex(x => x.Email).IsUnique();

        b.HasOne(x => x.Plan)
            .WithMany()
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasMany(x => x.Repertoires)
            .WithOne()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Materials)
            .WithOne()
            .HasForeignKey(m => m.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.PracticeSessions)
            .WithOne()
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(x => x.Repertoires).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(x => x.Materials).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(x => x.PracticeSessions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
