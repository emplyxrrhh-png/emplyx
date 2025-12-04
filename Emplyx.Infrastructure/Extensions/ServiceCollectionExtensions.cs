using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Infrastructure.Persistence;
using Emplyx.Infrastructure.Repositories;
using Emplyx.Infrastructure.Services;
using Emplyx.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Emplyx.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EmplyxDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'EmplyxDb' is not configured.");
        }

        services.AddDbContext<EmplyxDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(EmplyxDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure();
            });
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EmplyxDbContext>());

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IPermisoRepository, PermisoRepository>();
        services.AddScoped<IDelegacionRepository, DelegacionRepository>();
        services.AddScoped<IContextoRepository, ContextoRepository>();
        services.AddScoped<ILicenciaRepository, LicenciaRepository>();
        services.AddScoped<IClearanceRepository, ClearanceRepository>();
        services.AddScoped<IModuloRepository, ModuloRepository>();
        services.AddScoped<IDelegacionTemporalRepository, DelegacionTemporalRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ICentroTrabajoRepository, CentroTrabajoRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddSingleton<IFileStorageService, AzureBlobStorageService>();

        return services;
    }
}
