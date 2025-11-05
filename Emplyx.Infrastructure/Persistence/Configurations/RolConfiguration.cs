using Emplyx.Domain.Entities.Clearances;
using Emplyx.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.Descripcion)
            .HasMaxLength(500);

        builder.Property(r => r.IsSystem)
            .IsRequired();

        builder.Property(r => r.ClearanceId);

        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();

        builder.Property(r => r.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(r => r.Nombre)
            .IsUnique();

        builder.HasOne<Clearance>()
            .WithMany()
            .HasForeignKey(r => r.ClearanceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(r => r.Permisos)
            .HasField("_permisos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(r => r.Delegaciones)
            .HasField("_delegaciones")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
