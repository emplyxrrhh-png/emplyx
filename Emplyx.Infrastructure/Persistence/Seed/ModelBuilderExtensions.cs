using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Persistence.Seed;

internal static class ModelBuilderExtensions
{
    public static void ApplySeedData(this ModelBuilder modelBuilder)
    {
        SeedData.Configure(modelBuilder);
    }
}
