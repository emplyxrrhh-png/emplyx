using Emplyx.Domain.Entities.Modulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> builder)
    {
        builder.ToTable("Modulos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Codigo)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(m => m.Codigo)
            .IsUnique();

        builder.Property(m => m.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Descripcion)
            .HasMaxLength(500);

        builder.Property(m => m.EsCritico)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.CreatedAtUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedAtUtc)
            .IsRequired();
    }
}
