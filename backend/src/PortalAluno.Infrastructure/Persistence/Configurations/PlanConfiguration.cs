using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortalAluno.Domain.Entities;

namespace PortalAluno.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> b)
    {
        b.ToTable("plans");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        b.Property(x => x.SessionsPerMonth).IsRequired();
        b.Property(x => x.DurationMinutes).IsRequired();
        b.Property(x => x.Price).HasColumnType("numeric(10,2)").IsRequired();
        b.Property(x => x.Summary).HasMaxLength(400).IsRequired();
        b.Property(x => x.Features).HasMaxLength(2000).IsRequired();
        b.Property(x => x.DisplayOrder).IsRequired();

        b.HasIndex(x => x.Code).IsUnique();
    }
}
