using Emplyx.Domain.Entities.Contextos;
using Emplyx.Domain.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class UsuarioContextoConfiguration : IEntityTypeConfiguration<UsuarioContexto>
{
    public void Configure(EntityTypeBuilder<UsuarioContexto> builder)
    {
        builder.ToTable("UsuarioContextos");

        builder.HasKey(uc => new { uc.UsuarioId, uc.ContextoId });

        builder.Property(uc => uc.LinkedAtUtc)
            .IsRequired();

        builder.Property(uc => uc.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(uc => uc.ContextoId);

        builder.HasIndex(uc => uc.UsuarioId)
            .HasDatabaseName("IX_UsuarioContextos_Primary")
            .HasFilter("[IsPrimary] = 1");

        builder.HasOne<Usuario>()
            .WithMany(u => u.Contextos)
            .HasForeignKey(uc => uc.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Contexto>()
            .WithMany()
            .HasForeignKey(uc => uc.ContextoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
