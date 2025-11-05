using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Emplyx.Infrastructure.Persistence;

public sealed class EmplyxDbContextFactory : IDesignTimeDbContextFactory<EmplyxDbContext>
{
    public EmplyxDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EmplyxDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("EMPLYX_CONNECTION_STRING") ??
                               "Server=(localdb)\\MSSQLLocalDB;Database=Emplyx;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(EmplyxDbContext).Assembly.FullName);
        });

        return new EmplyxDbContext(optionsBuilder.Options);
    }
}
