using Emplyx.Domain.Entities.Licencias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class LicenciaConfiguration : IEntityTypeConfiguration<Licencia>
{
    public void Configure(EntityTypeBuilder<Licencia> builder)
    {
        builder.ToTable("Licencias");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Codigo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.EsTrial)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(l => l.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(l => l.LimiteUsuarios);

        builder.Property(l => l.InicioVigenciaUtc)
            .IsRequired();

        builder.Property(l => l.CreatedAtUtc)
            .IsRequired();

        builder.Property(l => l.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(l => l.Codigo)
            .IsUnique();

        builder.Navigation(l => l.Modulos)
            .HasField("_modulos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
