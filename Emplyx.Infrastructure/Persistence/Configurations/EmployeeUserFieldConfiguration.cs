using Emplyx.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeUserFieldConfiguration : IEntityTypeConfiguration<EmployeeUserField>
{
    public void Configure(EntityTypeBuilder<EmployeeUserField> builder)
    {
        builder.ToTable("EmployeeUserFields");

        builder.HasKey(uf => uf.Id);

        builder.Property(uf => uf.FieldDefinitionId).HasMaxLength(100).IsRequired();
        builder.Property(uf => uf.Value).HasMaxLength(4000);
    }
}
