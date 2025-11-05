using Emplyx.Domain.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(512);

        builder.Property(u => u.ExternalIdentityId)
            .HasMaxLength(200);

        builder.Property(u => u.PreferredContextoId);

        builder.Property(u => u.CreatedAtUtc)
            .IsRequired();

        builder.Property(u => u.UpdatedAtUtc)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(u => u.LastLoginAtUtc);

        builder.Property(u => u.LastPasswordChangeAtUtc);

        builder.OwnsOne(u => u.Perfil, perfil =>
        {
            perfil.Property(p => p.Nombres)
                .HasColumnName("PerfilNombres")
                .HasMaxLength(200);

            perfil.Property(p => p.Apellidos)
                .HasColumnName("PerfilApellidos")
                .HasMaxLength(200);

            perfil.Property(p => p.Departamento)
                .HasColumnName("PerfilDepartamento")
                .HasMaxLength(150);

            perfil.Property(p => p.Cargo)
                .HasColumnName("PerfilCargo")
                .HasMaxLength(150);

            perfil.Property(p => p.Telefono)
                .HasColumnName("PerfilTelefono")
                .HasMaxLength(50);
        });

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Navigation(u => u.Roles)
            .HasField("_roles");

        builder.Navigation(u => u.Contextos)
            .HasField("_contextos");

        builder.Navigation(u => u.Licencias)
            .HasField("_licencias")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Sesiones)
            .HasField("_sesiones")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Roles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Contextos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
