using Emplyx.Domain.Entities.CentrosTrabajo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class CentroTrabajoConfiguration : IEntityTypeConfiguration<CentroTrabajo>
{
    public void Configure(EntityTypeBuilder<CentroTrabajo> builder)
    {
        builder.ToTable("CentrosTrabajo");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.InternalId)
            .HasMaxLength(50);

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).HasColumnName("Address_Street");
            address.Property(a => a.ZipCode).HasMaxLength(20).HasColumnName("Address_ZipCode");
            address.Property(a => a.City).HasMaxLength(100).HasColumnName("Address_City");
            address.Property(a => a.Province).HasMaxLength(100).HasColumnName("Address_Province");
            address.Property(a => a.Country).HasMaxLength(100).HasColumnName("Address_Country");
        });

        builder.OwnsOne(c => c.Contact, contact =>
        {
            contact.Property(c => c.Name).HasMaxLength(200).HasColumnName("Contact_Name");
            contact.Property(c => c.Phone).HasMaxLength(50).HasColumnName("Contact_Phone");
            contact.Property(c => c.Email).HasMaxLength(200).HasColumnName("Contact_Email");
        });

        builder.Property(c => c.TimeZone).HasMaxLength(100);
        builder.Property(c => c.Language).HasMaxLength(10);
    }
}
