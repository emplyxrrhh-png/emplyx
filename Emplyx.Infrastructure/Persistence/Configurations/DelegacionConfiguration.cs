using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class DelegacionConfiguration : IEntityTypeConfiguration<Delegacion>
{
    public void Configure(EntityTypeBuilder<Delegacion> builder)
    {
        builder.ToTable("Delegaciones");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.Codigo)
            .HasMaxLength(50);

        builder.Property(d => d.Descripcion)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();

        builder.Property(d => d.UpdatedAtUtc)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(d => d.Codigo)
            .IsUnique()
            .HasFilter("[Codigo] IS NOT NULL");

        builder.HasOne<Delegacion>()
            .WithMany()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(d => d.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
