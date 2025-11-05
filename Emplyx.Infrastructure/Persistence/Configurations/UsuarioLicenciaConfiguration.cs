using Emplyx.Domain.Entities.Licencias;
using Emplyx.Domain.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class UsuarioLicenciaConfiguration : IEntityTypeConfiguration<UsuarioLicencia>
{
    public void Configure(EntityTypeBuilder<UsuarioLicencia> builder)
    {
        builder.ToTable("UsuarioLicencias");

        builder.HasKey(ul => new { ul.UsuarioId, ul.LicenciaId });

        builder.Property(ul => ul.AssignedAtUtc)
            .IsRequired();

        builder.HasOne<Usuario>()
            .WithMany(u => u.Licencias)
            .HasForeignKey(ul => ul.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Licencia>()
            .WithMany()
            .HasForeignKey(ul => ul.LicenciaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
