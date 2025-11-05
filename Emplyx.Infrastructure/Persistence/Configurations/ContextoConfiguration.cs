using Emplyx.Domain.Entities.Clearances;
using Emplyx.Domain.Entities.Contextos;
using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Entities.Licencias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class ContextoConfiguration : IEntityTypeConfiguration<Contexto>
{
    public void Configure(EntityTypeBuilder<Contexto> builder)
    {
        builder.ToTable("Contextos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Clave)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(c => c.Clave)
            .IsUnique();

        builder.HasOne<Delegacion>()
            .WithMany()
            .HasForeignKey(c => c.DelegacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Licencia>()
            .WithMany()
            .HasForeignKey(c => c.LicenciaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Clearance>()
            .WithMany()
            .HasForeignKey(c => c.ClearanceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(c => c.Modulos)
            .HasField("_modulos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
