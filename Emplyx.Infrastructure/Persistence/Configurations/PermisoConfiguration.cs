using Emplyx.Domain.Entities.Permisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permisos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Codigo)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.ModuloId)
            .IsRequired();

        builder.Property(p => p.Categoria)
            .HasMaxLength(150);

        builder.Property(p => p.EsCritico)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(p => p.Codigo)
            .IsUnique();

        builder.HasIndex(p => p.ModuloId);

        builder.HasOne<Emplyx.Domain.Entities.Modulos.Modulo>()
            .WithMany()
            .HasForeignKey(p => p.ModuloId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
