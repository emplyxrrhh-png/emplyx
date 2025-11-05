using Emplyx.Domain.Entities.Contextos;
using Emplyx.Domain.Entities.Modulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class ContextoModuloConfiguration : IEntityTypeConfiguration<ContextoModulo>
{
    public void Configure(EntityTypeBuilder<ContextoModulo> builder)
    {
        builder.ToTable("ContextoModulos");

        builder.HasKey(cm => new { cm.ContextoId, cm.ModuloId });

        builder.Property(cm => cm.HabilitadoDesdeUtc)
            .IsRequired();

        builder.Property(cm => cm.HabilitadoHastaUtc);

        builder.HasOne<Contexto>()
            .WithMany(c => c.Modulos)
            .HasForeignKey(cm => cm.ContextoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Modulo>()
            .WithMany()
            .HasForeignKey(cm => cm.ModuloId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
