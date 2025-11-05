using Emplyx.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

internal sealed class SeedVersionConfiguration : IEntityTypeConfiguration<SeedVersion>
{
    public void Configure(EntityTypeBuilder<SeedVersion> builder)
    {
        builder.ToTable("SeedVersions");

        builder.HasKey(sv => sv.Id);

        builder.Property(sv => sv.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sv => sv.Version)
            .IsRequired();

        builder.Property(sv => sv.AppliedAtUtc)
            .IsRequired();

        builder.HasIndex(sv => sv.Key)
            .IsUnique();
    }
}
