using Emplyx.Application.Abstractions;
using Emplyx.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Emplyx.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRolService, RolService>();
        services.AddScoped<IDelegacionService, DelegacionService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ICentroTrabajoService, CentroTrabajoService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}
