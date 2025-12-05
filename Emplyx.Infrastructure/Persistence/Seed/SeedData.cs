using Emplyx.Domain.Entities.Clearances;
using Emplyx.Domain.Entities.Contextos;
using Emplyx.Domain.Entities.Delegaciones;
using Emplyx.Domain.Entities.Licencias;
using Emplyx.Domain.Entities.Modulos;
using Emplyx.Domain.Entities.Permisos;
using Emplyx.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Persistence.Seed;

internal static class SeedData
{
    private static readonly DateTime SeedTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly Guid ClearancePublicaId = Guid.Parse("0ad1bc87-5e61-4258-8f51-2305b7c5401d");
    internal static readonly Guid ClearanceInternaId = Guid.Parse("25dfbb4d-8f45-4a54-958f-07daa4619052");
    internal static readonly Guid ClearanceRestringidaId = Guid.Parse("5d7a9d25-c4b2-4b93-93c6-a39c47da1f40");

    internal static readonly Guid ModuloSeguridadId = Guid.Parse("6f09c219-4c23-4c89-bf13-4f08bc3d24a0");
    internal static readonly Guid ModuloOperacionesId = Guid.Parse("ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5");
    internal static readonly Guid ModuloLicenciamientoId = Guid.Parse("3a6f9950-9e50-4a22-b417-7fb7a57de5fe");

    internal static readonly Guid LicenciaBaseId = Guid.Parse("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29");

    internal static readonly Guid DelegacionCentralId = Guid.Parse("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2");
    internal static readonly Guid DelegacionNorteId = Guid.Parse("d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424");
    internal static readonly Guid DelegacionSurId = Guid.Parse("e4d8d94a-9084-46c3-99c1-1b4ef76e6a19");

    internal static readonly Guid ContextoDefaultId = Guid.Parse("caa6d616-f237-4d2b-a3f1-bf54cd508c35");

    internal static readonly Guid RolAdminGlobalId = Guid.Parse("3fbdede0-0773-4cb6-9881-9350d0f4955e");
    internal static readonly Guid RolGestorDelegacionId = Guid.Parse("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4");
    internal static readonly Guid RolOperadorId = Guid.Parse("a9d6d2dc-136f-42e4-9b77-75568128ed5f");

    internal static readonly Guid PermisoGestionUsuariosId = Guid.Parse("f1d2c6fe-39f8-4832-9e2d-468f4bea7c31");
    internal static readonly Guid PermisoLecturaUsuariosId = Guid.Parse("8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c");
    internal static readonly Guid PermisoGestionRolesId = Guid.Parse("76d8cc5f-4224-4ef3-a594-984cd5c50a62");
    internal static readonly Guid PermisoGestionContextosId = Guid.Parse("d854df08-26d2-4af7-98d3-49816d23c01f");
    internal static readonly Guid PermisoLecturaContextosId = Guid.Parse("dc6c8079-0af4-4a35-86d6-c9f6a4541bc9");
    internal static readonly Guid PermisoGestionLicenciasId = Guid.Parse("c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1");

    internal static readonly Guid SeedVersionId = Guid.Parse("d6e36d75-6f90-4d90-b1b4-0d423b99d8f1");

    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clearance>().HasData(
            new
            {
                Id = ClearancePublicaId,
                Nombre = "Publico",
                Nivel = 1,
                Descripcion = "Acceso general sin restricciones.",
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = ClearanceInternaId,
                Nombre = "Interno",
                Nivel = 2,
                Descripcion = "Acceso para personal interno y socios de confianza.",
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = ClearanceRestringidaId,
                Nombre = "Restringido",
                Nivel = 3,
                Descripcion = "Acceso reservado a datos confidenciales y criticos.",
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<Modulo>().HasData(
            new
            {
                Id = ModuloSeguridadId,
                Codigo = "SEGURIDAD",
                Nombre = "Seguridad",
                Descripcion = "Gestion de usuarios, roles y permisos.",
                EsCritico = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = ModuloOperacionesId,
                Codigo = "OPERACIONES",
                Nombre = "Operaciones",
                Descripcion = "Operativa diaria y contextos de trabajo.",
                EsCritico = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = ModuloLicenciamientoId,
                Codigo = "LICENCIAS",
                Nombre = "Licenciamiento",
                Descripcion = "Administracion de licencias y modulos contratados.",
                EsCritico = false,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<Licencia>().HasData(
            new
            {
                Id = LicenciaBaseId,
                Codigo = "CORE-PRO",
                Nombre = "Licencia Profesional Core",
                InicioVigenciaUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                FinVigenciaUtc = new DateTime(2030, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                LimiteUsuarios = 500,
                EsTrial = false,
                IsRevoked = false,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<LicenciaModulo>().HasData(
            new { LicenciaId = LicenciaBaseId, ModuloId = ModuloSeguridadId, LinkedAtUtc = SeedTimestamp },
            new { LicenciaId = LicenciaBaseId, ModuloId = ModuloOperacionesId, LinkedAtUtc = SeedTimestamp },
            new { LicenciaId = LicenciaBaseId, ModuloId = ModuloLicenciamientoId, LinkedAtUtc = SeedTimestamp });

        modelBuilder.Entity<Delegacion>().HasData(
            new
            {
                Id = DelegacionCentralId,
                Nombre = "Delegacion Central",
                Codigo = "CENTRAL",
                Descripcion = "Sede central corporativa.",
                ParentId = (Guid?)null,
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = DelegacionNorteId,
                Nombre = "Delegacion Regional Norte",
                Codigo = "NORTE",
                Descripcion = "Operaciones regionales zona norte.",
                ParentId = DelegacionCentralId,
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = DelegacionSurId,
                Nombre = "Delegacion Regional Sur",
                Codigo = "SUR",
                Descripcion = "Operaciones regionales zona sur.",
                ParentId = DelegacionCentralId,
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<Contexto>().HasData(
            new
            {
                Id = ContextoDefaultId,
                Clave = "DEFAULT",
                Nombre = "Contexto Principal",
                Descripcion = "Contexto operativo principal del sistema.",
                DelegacionId = DelegacionCentralId,
                LicenciaId = LicenciaBaseId,
                ClearanceId = ClearanceInternaId,
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<ContextoModulo>().HasData(
            new { ContextoId = ContextoDefaultId, ModuloId = ModuloSeguridadId, HabilitadoDesdeUtc = SeedTimestamp, HabilitadoHastaUtc = (DateTime?)null },
            new { ContextoId = ContextoDefaultId, ModuloId = ModuloOperacionesId, HabilitadoDesdeUtc = SeedTimestamp, HabilitadoHastaUtc = (DateTime?)null },
            new { ContextoId = ContextoDefaultId, ModuloId = ModuloLicenciamientoId, HabilitadoDesdeUtc = SeedTimestamp, HabilitadoHastaUtc = (DateTime?)null });

        modelBuilder.Entity<Rol>().HasData(
            new
            {
                Id = RolAdminGlobalId,
                Nombre = "Administrador Global",
                Descripcion = "Acceso total a la plataforma.",
                IsSystem = true,
                ClearanceId = ClearanceRestringidaId,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = RolGestorDelegacionId,
                Nombre = "Gestor de Delegacion",
                Descripcion = "Gestiona recursos dentro de su delegacion.",
                IsSystem = false,
                ClearanceId = ClearanceInternaId,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = RolOperadorId,
                Nombre = "Operador",
                Descripcion = "Operativa diaria de la plataforma.",
                IsSystem = false,
                ClearanceId = ClearancePublicaId,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<Permiso>().HasData(
            new
            {
                Id = PermisoGestionUsuariosId,
                Codigo = "SEGURIDAD.USUARIOS.GESTIONAR",
                Nombre = "Gestionar usuarios",
                ModuloId = ModuloSeguridadId,
                Categoria = "Usuarios",
                EsCritico = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = PermisoLecturaUsuariosId,
                Codigo = "SEGURIDAD.USUARIOS.VER",
                Nombre = "Ver usuarios",
                ModuloId = ModuloSeguridadId,
                Categoria = "Usuarios",
                EsCritico = false,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = PermisoGestionRolesId,
                Codigo = "SEGURIDAD.ROLES.GESTIONAR",
                Nombre = "Gestionar roles",
                ModuloId = ModuloSeguridadId,
                Categoria = "Roles",
                EsCritico = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = PermisoGestionContextosId,
                Codigo = "OPERACIONES.CONTEXTOS.GESTIONAR",
                Nombre = "Gestionar contextos",
                ModuloId = ModuloOperacionesId,
                Categoria = "Contextos",
                EsCritico = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = PermisoLecturaContextosId,
                Codigo = "OPERACIONES.CONTEXTOS.VER",
                Nombre = "Ver contextos",
                ModuloId = ModuloOperacionesId,
                Categoria = "Contextos",
                EsCritico = false,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            },
            new
            {
                Id = PermisoGestionLicenciasId,
                Codigo = "LICENCIAS.MAESTRO.GESTIONAR",
                Nombre = "Gestionar licencias",
                ModuloId = ModuloLicenciamientoId,
                Categoria = "Licencias",
                EsCritico = true,
                CreatedAtUtc = SeedTimestamp,
                UpdatedAtUtc = SeedTimestamp
            });

        modelBuilder.Entity<RolPermiso>().HasData(
            new { RolId = RolAdminGlobalId, PermisoId = PermisoGestionUsuariosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolAdminGlobalId, PermisoId = PermisoLecturaUsuariosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolAdminGlobalId, PermisoId = PermisoGestionRolesId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolAdminGlobalId, PermisoId = PermisoGestionContextosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolAdminGlobalId, PermisoId = PermisoLecturaContextosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolAdminGlobalId, PermisoId = PermisoGestionLicenciasId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolGestorDelegacionId, PermisoId = PermisoLecturaUsuariosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolGestorDelegacionId, PermisoId = PermisoGestionContextosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolGestorDelegacionId, PermisoId = PermisoLecturaContextosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolOperadorId, PermisoId = PermisoLecturaUsuariosId, GrantedAtUtc = SeedTimestamp },
            new { RolId = RolOperadorId, PermisoId = PermisoLecturaContextosId, GrantedAtUtc = SeedTimestamp });

        modelBuilder.Entity<DelegacionRol>().HasData(
            new { DelegacionId = DelegacionCentralId, RolId = RolAdminGlobalId, LinkedAtUtc = SeedTimestamp },
            new { DelegacionId = DelegacionCentralId, RolId = RolGestorDelegacionId, LinkedAtUtc = SeedTimestamp },
            new { DelegacionId = DelegacionNorteId, RolId = RolGestorDelegacionId, LinkedAtUtc = SeedTimestamp },
            new { DelegacionId = DelegacionSurId, RolId = RolGestorDelegacionId, LinkedAtUtc = SeedTimestamp },
            new { DelegacionId = DelegacionCentralId, RolId = RolOperadorId, LinkedAtUtc = SeedTimestamp },
            new { DelegacionId = DelegacionNorteId, RolId = RolOperadorId, LinkedAtUtc = SeedTimestamp },
            new { DelegacionId = DelegacionSurId, RolId = RolOperadorId, LinkedAtUtc = SeedTimestamp });

        modelBuilder.Entity<SeedVersion>().HasData(
            new
            {
                Id = SeedVersionId,
                Key = "CoreAuthorizationSeed",
                Version = 1,
                AppliedAtUtc = SeedTimestamp
            });
    }
}
