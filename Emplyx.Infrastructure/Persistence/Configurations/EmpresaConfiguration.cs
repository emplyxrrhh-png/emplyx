using Emplyx.Domain.Entities.Empresas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.RazonSocial)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.CIF)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Direccion)
            .HasMaxLength(500);

        builder.Property(e => e.Telefono)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.Property(e => e.Web)
            .HasMaxLength(200);

        builder.Property(e => e.Pais)
            .HasMaxLength(100);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);
    }
}
