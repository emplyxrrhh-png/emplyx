using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class DelegacionRolConfiguration : IEntityTypeConfiguration<DelegacionRol>
{
    public void Configure(EntityTypeBuilder<DelegacionRol> builder)
    {
        builder.ToTable("DelegacionRoles");

        builder.HasKey(dr => new { dr.DelegacionId, dr.RolId });

        builder.Property(dr => dr.LinkedAtUtc)
            .IsRequired();

        builder.HasIndex(dr => dr.RolId);

        builder.HasOne<Delegacion>()
            .WithMany(d => d.Roles)
            .HasForeignKey(dr => dr.DelegacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Rol>()
            .WithMany(r => r.Delegaciones)
            .HasForeignKey(dr => dr.RolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
