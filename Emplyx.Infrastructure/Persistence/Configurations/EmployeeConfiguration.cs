using Emplyx.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Apellidos).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Alias).HasMaxLength(100);
        builder.Property(e => e.GroupName).HasMaxLength(100);
        builder.Property(e => e.Type).HasMaxLength(50);
        builder.Property(e => e.Status).HasMaxLength(50);
        builder.Property(e => e.Image).HasMaxLength(500);

        builder.Property(e => e.ContractType).HasMaxLength(50);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.Idioma).HasMaxLength(10);

        builder.Property(e => e.IDAccessGroup).HasMaxLength(50);
        builder.Property(e => e.BiometricID).HasMaxLength(50);
        
        builder.Property(e => e.WebLogin).HasMaxLength(100);
        builder.Property(e => e.WebPassword).HasMaxLength(200);

        builder.HasMany(e => e.UserFields)
            .WithOne()
            .HasForeignKey(uf => uf.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
