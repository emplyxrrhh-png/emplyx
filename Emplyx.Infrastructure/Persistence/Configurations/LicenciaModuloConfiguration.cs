using Emplyx.Domain.Entities.Licencias;
using Emplyx.Domain.Entities.Modulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class LicenciaModuloConfiguration : IEntityTypeConfiguration<LicenciaModulo>
{
    public void Configure(EntityTypeBuilder<LicenciaModulo> builder)
    {
        builder.ToTable("LicenciaModulos");

        builder.HasKey(lm => new { lm.LicenciaId, lm.ModuloId });

        builder.Property(lm => lm.LinkedAtUtc)
            .IsRequired();

        builder.HasOne<Licencia>()
            .WithMany(l => l.Modulos)
            .HasForeignKey(lm => lm.LicenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Modulo>()
            .WithMany()
            .HasForeignKey(lm => lm.ModuloId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
