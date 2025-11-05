using Emplyx.Domain.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class UsuarioSesionConfiguration : IEntityTypeConfiguration<UsuarioSesion>
{
    public void Configure(EntityTypeBuilder<UsuarioSesion> builder)
    {
        builder.ToTable("UsuarioSesiones");

        builder.HasKey(us => us.Id);

        builder.Property(us => us.Device)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(us => us.IpAddress)
            .HasMaxLength(45);

        builder.Property(us => us.CreatedAtUtc)
            .IsRequired();

        builder.Property(us => us.ExpiresAtUtc)
            .IsRequired();

        builder.Property(us => us.IsActive)
            .IsRequired();

        builder.Property(us => us.ClosedAtUtc);

        builder.HasIndex(us => new { us.UsuarioId, us.IsActive });

        builder.HasOne<Usuario>()
            .WithMany(u => u.Sesiones)
            .HasForeignKey(us => us.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
