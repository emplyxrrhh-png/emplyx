using Emplyx.Domain.Entities.DelegacionesTemporales;
using Emplyx.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class DelegacionTemporalRolConfiguration : IEntityTypeConfiguration<DelegacionTemporalRol>
{
    public void Configure(EntityTypeBuilder<DelegacionTemporalRol> builder)
    {
        builder.ToTable("DelegacionTemporalRoles");

        builder.HasKey(dtr => new { dtr.DelegacionTemporalId, dtr.RolId });

        builder.HasOne<DelegacionTemporal>()
            .WithMany(dt => dt.Roles)
            .HasForeignKey(dtr => dtr.DelegacionTemporalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Rol>()
            .WithMany()
            .HasForeignKey(dtr => dtr.RolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
