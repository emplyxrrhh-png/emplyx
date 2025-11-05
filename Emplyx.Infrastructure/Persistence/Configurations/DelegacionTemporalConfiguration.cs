using Emplyx.Domain.Entities.DelegacionesTemporales;
using Emplyx.Domain.Entities.Roles;
using Emplyx.Domain.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class DelegacionTemporalConfiguration : IEntityTypeConfiguration<DelegacionTemporal>
{
    public void Configure(EntityTypeBuilder<DelegacionTemporal> builder)
    {
        builder.ToTable("DelegacionesTemporales");

        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.DeleganteId)
            .IsRequired();

        builder.Property(dt => dt.DelegadoId)
            .IsRequired();

        builder.Property(dt => dt.InicioUtc)
            .IsRequired();

        builder.Property(dt => dt.FinUtc)
            .IsRequired();

        builder.Property(dt => dt.Estado)
            .IsRequired();

        builder.Property(dt => dt.AplicaTodosLosRoles)
            .IsRequired();

        builder.Property(dt => dt.AprobadaMfa)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dt => dt.MetodoMfa)
            .HasMaxLength(100);

        builder.Property(dt => dt.CreatedAtUtc)
            .IsRequired();

        builder.Property(dt => dt.UpdatedAtUtc)
            .IsRequired();

        builder.Property(dt => dt.RevocadaUtc);
        builder.Property(dt => dt.ExpiradaUtc);

        builder.HasIndex(dt => new { dt.DeleganteId, dt.Estado });
        builder.HasIndex(dt => new { dt.DelegadoId, dt.Estado });

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(dt => dt.DeleganteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(dt => dt.DelegadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(dt => dt.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
