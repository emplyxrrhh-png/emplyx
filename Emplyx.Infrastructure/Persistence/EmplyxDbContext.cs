using Emplyx.Domain.Entities.Clearances;
using Emplyx.Domain.Entities.Contextos;
using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Entities.DelegacionesTemporales;
using Emplyx.Domain.Entities.Empresas;
using Emplyx.Domain.Entities.Licencias;
using Emplyx.Domain.Entities.Modulos;
using Emplyx.Domain.Entities.Permisos;
using Emplyx.Domain.Entities.Roles;
using Emplyx.Domain.Entities.Usuarios;
using Emplyx.Domain.Entities.Tenants;
using Emplyx.Domain.Entities.CentrosTrabajo;
using Emplyx.Domain.Entities.Employees;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Emplyx.Infrastructure.Persistence;

public sealed class EmplyxDbContext : DbContext, IUnitOfWork
{
    public EmplyxDbContext(DbContextOptions<EmplyxDbContext> options)
        : base(options)
    {
    }

    public DbSet<CentroTrabajo> CentrosTrabajo => Set<CentroTrabajo>();

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeUserField> EmployeeUserFields => Set<EmployeeUserField>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();

    public DbSet<UsuarioContexto> UsuarioContextos => Set<UsuarioContexto>();

    public DbSet<UsuarioLicencia> UsuarioLicencias => Set<UsuarioLicencia>();

    public DbSet<UsuarioSesion> UsuarioSesiones => Set<UsuarioSesion>();

    public DbSet<Rol> Roles => Set<Rol>();

    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();

    public DbSet<Permiso> Permisos => Set<Permiso>();

    public DbSet<Modulo> Modulos => Set<Modulo>();

    public DbSet<Delegacion> Delegaciones => Set<Delegacion>();

    public DbSet<DelegacionRol> DelegacionRoles => Set<DelegacionRol>();

    public DbSet<DelegacionTemporal> DelegacionesTemporales => Set<DelegacionTemporal>();

    public DbSet<DelegacionTemporalRol> DelegacionTemporalRoles => Set<DelegacionTemporalRol>();

    public DbSet<Contexto> Contextos => Set<Contexto>();

    public DbSet<Empresa> Empresas => Set<Empresa>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<ContextoModulo> ContextoModulos => Set<ContextoModulo>();

    public DbSet<Licencia> Licencias => Set<Licencia>();

    public DbSet<LicenciaModulo> LicenciaModulos => Set<LicenciaModulo>();

    public DbSet<Clearance> Clearances => Set<Clearance>();

    internal DbSet<SeedVersion> SeedVersions => Set<SeedVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmplyxDbContext).Assembly);
        modelBuilder.ApplySeedData();
    }

    public async Task<IDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return transaction;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        base.SaveChangesAsync(cancellationToken);

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await base.DisposeAsync();
    }
}
