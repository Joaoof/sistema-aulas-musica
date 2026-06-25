using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalAluno.Domain.Entities;

namespace PortalAluno.Infrastructure.Persistence.Configurations;

public class RepertoireConfiguration : IEntityTypeConfiguration<Repertoire>
{
    public void Configure(EntityTypeBuilder<Repertoire> b)
    {
        b.ToTable("repertoires");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever(); // Id gerado no domínio

        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Composer).HasMaxLength(160).IsRequired();
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.VideoUrl).HasMaxLength(1000);
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasIndex(x => x.StudentId);
    }
}
