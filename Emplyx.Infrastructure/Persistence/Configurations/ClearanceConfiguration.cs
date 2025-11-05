using Emplyx.Domain.Entities.Clearances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class ClearanceConfiguration : IEntityTypeConfiguration<Clearance>
{
    public void Configure(EntityTypeBuilder<Clearance> builder)
    {
        builder.ToTable("Clearances");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Nivel)
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

        builder.HasIndex(c => c.Nivel)
            .IsUnique();
    }
}
