using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalAluno.Domain.Entities;

namespace PortalAluno.Infrastructure.Persistence.Configurations;

public class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSession>
{
    public void Configure(EntityTypeBuilder<PracticeSession> b)
    {
        b.ToTable("practice_sessions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever(); // Id gerado no domínio

        b.Property(x => x.Date).HasColumnType("date").IsRequired();
        b.Property(x => x.Bpm).IsRequired();

        b.HasIndex(x => new { x.StudentId, x.Date });
    }
}
