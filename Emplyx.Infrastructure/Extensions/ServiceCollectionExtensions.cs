using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Infrastructure.Persistence;
using Emplyx.Infrastructure.Repositories;
using Emplyx.Infrastructure.Services;
using Emplyx.Infrastructure.Security;
using Emplyx.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
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

        services.AddScoped<ICentroTrabajoRepository, CentroTrabajoRepository>();
        services.AddScoped<IClearanceRepository, ClearanceRepository>();
        services.AddScoped<IContextoRepository, ContextoRepository>();
        services.AddScoped<IDelegacionRepository, DelegacionRepository>();
        services.AddScoped<IDelegacionTemporalRepository, DelegacionTemporalRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<ILicenciaRepository, LicenciaRepository>();
        services.AddScoped<IModuloRepository, ModuloRepository>();
        services.AddScoped<IPermisoRepository, PermisoRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddSingleton<IFileStorageService, AzureBlobStorageService>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}
