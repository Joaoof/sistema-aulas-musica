using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalAluno.Domain.Entities;

namespace PortalAluno.Infrastructure.Persistence.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> b)
    {
        b.ToTable("materials");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever(); // Id gerado no domínio

        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.ExternalUrl).HasMaxLength(1000).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasIndex(x => x.StudentId);
    }
}
