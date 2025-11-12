using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Emplyx.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clearances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clearances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Delegaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delegaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Delegaciones_Delegaciones_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Delegaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Licencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InicioVigenciaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinVigenciaUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LimiteUsuarios = table.Column<int>(type: "int", nullable: true),
                    EsTrial = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EsCritico = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeedVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeedVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ClearanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ExternalIdentityId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreferredContextoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordChangeAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerfilNombres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerfilApellidos = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerfilDepartamento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PerfilCargo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PerfilTelefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    ClearanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Clearances_ClearanceId",
                        column: x => x.ClearanceId,
                        principalTable: "Clearances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Contextos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DelegacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClearanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contextos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contextos_Clearances_ClearanceId",
                        column: x => x.ClearanceId,
                        principalTable: "Clearances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contextos_Delegaciones_DelegacionId",
                        column: x => x.DelegacionId,
                        principalTable: "Delegaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contextos_Licencias_LicenciaId",
                        column: x => x.LicenciaId,
                        principalTable: "Licencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LicenciaModulos",
                columns: table => new
                {
                    LicenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuloId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenciaModulos", x => new { x.LicenciaId, x.ModuloId });
                    table.ForeignKey(
                        name: "FK_LicenciaModulos_Licencias_LicenciaId",
                        column: x => x.LicenciaId,
                        principalTable: "Licencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenciaModulos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModuloId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EsCritico = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permisos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DelegacionesTemporales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleganteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DelegadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InicioUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AplicaTodosLosRoles = table.Column<bool>(type: "bit", nullable: false),
                    AprobadaMfa = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MetodoMfa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevocadaUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiradaUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegacionesTemporales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DelegacionesTemporales_Usuarios_DelegadoId",
                        column: x => x.DelegadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DelegacionesTemporales_Usuarios_DeleganteId",
                        column: x => x.DeleganteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioLicencias",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioLicencias", x => new { x.UsuarioId, x.LicenciaId });
                    table.ForeignKey(
                        name: "FK_UsuarioLicencias_Licencias_LicenciaId",
                        column: x => x.LicenciaId,
                        principalTable: "Licencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioLicencias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSesiones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Device = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSesiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioSesiones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DelegacionRoles",
                columns: table => new
                {
                    DelegacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegacionRoles", x => new { x.DelegacionId, x.RolId });
                    table.ForeignKey(
                        name: "FK_DelegacionRoles_Delegaciones_DelegacionId",
                        column: x => x.DelegacionId,
                        principalTable: "Delegaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegacionRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContextoModulos",
                columns: table => new
                {
                    ContextoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuloId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabilitadoDesdeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HabilitadoHastaUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContextoModulos", x => new { x.ContextoId, x.ModuloId });
                    table.ForeignKey(
                        name: "FK_ContextoModulos_Contextos_ContextoId",
                        column: x => x.ContextoId,
                        principalTable: "Contextos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContextoModulos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioContextos",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContextoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioContextos", x => new { x.UsuarioId, x.ContextoId });
                    table.ForeignKey(
                        name: "FK_UsuarioContextos_Contextos_ContextoId",
                        column: x => x.ContextoId,
                        principalTable: "Contextos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioContextos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermisoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => new { x.RolId, x.PermisoId });
                    table.ForeignKey(
                        name: "FK_RolPermisos_Permisos_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "Permisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DelegacionTemporalRoles",
                columns: table => new
                {
                    DelegacionTemporalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegacionTemporalRoles", x => new { x.DelegacionTemporalId, x.RolId });
                    table.ForeignKey(
                        name: "FK_DelegacionTemporalRoles_DelegacionesTemporales_DelegacionTemporalId",
                        column: x => x.DelegacionTemporalId,
                        principalTable: "DelegacionesTemporales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegacionTemporalRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Clearances",
                columns: new[] { "Id", "CreatedAtUtc", "Descripcion", "IsActive", "Nivel", "Nombre", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("0ad1bc87-5e61-4258-8f51-2305b7c5401d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acceso general sin restricciones.", true, 1, "Publico", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("25dfbb4d-8f45-4a54-958f-07daa4619052"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acceso para personal interno y socios de confianza.", true, 2, "Interno", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5d7a9d25-c4b2-4b93-93c6-a39c47da1f40"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acceso reservado a datos confidenciales y criticos.", true, 3, "Restringido", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Delegaciones",
                columns: new[] { "Id", "Codigo", "CreatedAtUtc", "Descripcion", "IsActive", "Nombre", "ParentId", "UpdatedAtUtc" },
                values: new object[] { new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), "CENTRAL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sede central corporativa.", true, "Delegacion Central", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Licencias",
                columns: new[] { "Id", "Codigo", "CreatedAtUtc", "FinVigenciaUtc", "InicioVigenciaUtc", "LimiteUsuarios", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), "CORE-PRO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2030, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 500, "Licencia Profesional Core", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Id", "Codigo", "CreatedAtUtc", "Descripcion", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("3a6f9950-9e50-4a22-b417-7fb7a57de5fe"), "LICENCIAS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administracion de licencias y modulos contratados.", "Licenciamiento", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Id", "Codigo", "CreatedAtUtc", "Descripcion", "EsCritico", "Nombre", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("6f09c219-4c23-4c89-bf13-4f08bc3d24a0"), "SEGURIDAD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestion de usuarios, roles y permisos.", true, "Seguridad", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5"), "OPERACIONES", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operativa diaria y contextos de trabajo.", true, "Operaciones", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SeedVersions",
                columns: new[] { "Id", "AppliedAtUtc", "Key", "Version" },
                values: new object[] { new Guid("d6e36d75-6f90-4d90-b1b4-0d423b99d8f1"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CoreAuthorizationSeed", 1 });

            migrationBuilder.InsertData(
                table: "Contextos",
                columns: new[] { "Id", "Clave", "ClearanceId", "CreatedAtUtc", "DelegacionId", "Descripcion", "IsActive", "LicenciaId", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), "DEFAULT", new Guid("25dfbb4d-8f45-4a54-958f-07daa4619052"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), "Contexto operativo principal del sistema.", true, new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), "Contexto Principal", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Delegaciones",
                columns: new[] { "Id", "Codigo", "CreatedAtUtc", "Descripcion", "IsActive", "Nombre", "ParentId", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424"), "NORTE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operaciones regionales zona norte.", true, "Delegacion Regional Norte", new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e4d8d94a-9084-46c3-99c1-1b4ef76e6a19"), "SUR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operaciones regionales zona sur.", true, "Delegacion Regional Sur", new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "LicenciaModulos",
                columns: new[] { "LicenciaId", "ModuloId", "LinkedAtUtc" },
                values: new object[,]
                {
                    { new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), new Guid("3a6f9950-9e50-4a22-b417-7fb7a57de5fe"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), new Guid("6f09c219-4c23-4c89-bf13-4f08bc3d24a0"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), new Guid("ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Categoria", "Codigo", "CreatedAtUtc", "EsCritico", "ModuloId", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("76d8cc5f-4224-4ef3-a594-984cd5c50a62"), "Roles", "SEGURIDAD.ROLES.GESTIONAR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("6f09c219-4c23-4c89-bf13-4f08bc3d24a0"), "Gestionar roles", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Categoria", "Codigo", "CreatedAtUtc", "ModuloId", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c"), "Usuarios", "SEGURIDAD.USUARIOS.VER", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6f09c219-4c23-4c89-bf13-4f08bc3d24a0"), "Ver usuarios", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Categoria", "Codigo", "CreatedAtUtc", "EsCritico", "ModuloId", "Nombre", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1"), "Licencias", "LICENCIAS.MAESTRO.GESTIONAR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("3a6f9950-9e50-4a22-b417-7fb7a57de5fe"), "Gestionar licencias", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d854df08-26d2-4af7-98d3-49816d23c01f"), "Contextos", "OPERACIONES.CONTEXTOS.GESTIONAR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5"), "Gestionar contextos", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Categoria", "Codigo", "CreatedAtUtc", "ModuloId", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("dc6c8079-0af4-4a35-86d6-c9f6a4541bc9"), "Contextos", "OPERACIONES.CONTEXTOS.VER", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5"), "Ver contextos", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Categoria", "Codigo", "CreatedAtUtc", "EsCritico", "ModuloId", "Nombre", "UpdatedAtUtc" },
                values: new object[] { new Guid("f1d2c6fe-39f8-4832-9e2d-468f4bea7c31"), "Usuarios", "SEGURIDAD.USUARIOS.GESTIONAR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("6f09c219-4c23-4c89-bf13-4f08bc3d24a0"), "Gestionar usuarios", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ClearanceId", "CreatedAtUtc", "Descripcion", "IsSystem", "Nombre", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new Guid("5d7a9d25-c4b2-4b93-93c6-a39c47da1f40"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acceso total a la plataforma.", true, "Administrador Global", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new Guid("25dfbb4d-8f45-4a54-958f-07daa4619052"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestiona recursos dentro de su delegacion.", false, "Gestor de Delegacion", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a9d6d2dc-136f-42e4-9b77-75568128ed5f"), new Guid("0ad1bc87-5e61-4258-8f51-2305b7c5401d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operativa diaria de la plataforma.", false, "Operador", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "ContextoModulos",
                columns: new[] { "ContextoId", "ModuloId", "HabilitadoDesdeUtc", "HabilitadoHastaUtc" },
                values: new object[,]
                {
                    { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("3a6f9950-9e50-4a22-b417-7fb7a57de5fe"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("6f09c219-4c23-4c89-bf13-4f08bc3d24a0"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "DelegacionRoles",
                columns: new[] { "DelegacionId", "RolId", "LinkedAtUtc" },
                values: new object[,]
                {
                    { new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8f9033a5-436b-4e1f-9d05-21c27c6c3bf2"), new Guid("a9d6d2dc-136f-42e4-9b77-75568128ed5f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424"), new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424"), new Guid("a9d6d2dc-136f-42e4-9b77-75568128ed5f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e4d8d94a-9084-46c3-99c1-1b4ef76e6a19"), new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e4d8d94a-9084-46c3-99c1-1b4ef76e6a19"), new Guid("a9d6d2dc-136f-42e4-9b77-75568128ed5f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RolPermisos",
                columns: new[] { "PermisoId", "RolId", "GrantedAtUtc" },
                values: new object[,]
                {
                    { new Guid("76d8cc5f-4224-4ef3-a594-984cd5c50a62"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d854df08-26d2-4af7-98d3-49816d23c01f"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dc6c8079-0af4-4a35-86d6-c9f6a4541bc9"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f1d2c6fe-39f8-4832-9e2d-468f4bea7c31"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c"), new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d854df08-26d2-4af7-98d3-49816d23c01f"), new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dc6c8079-0af4-4a35-86d6-c9f6a4541bc9"), new Guid("a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c"), new Guid("a9d6d2dc-136f-42e4-9b77-75568128ed5f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dc6c8079-0af4-4a35-86d6-c9f6a4541bc9"), new Guid("a9d6d2dc-136f-42e4-9b77-75568128ed5f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clearances_Nivel",
                table: "Clearances",
                column: "Nivel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContextoModulos_ModuloId",
                table: "ContextoModulos",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Contextos_Clave",
                table: "Contextos",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contextos_ClearanceId",
                table: "Contextos",
                column: "ClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Contextos_DelegacionId",
                table: "Contextos",
                column: "DelegacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Contextos_LicenciaId",
                table: "Contextos",
                column: "LicenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegaciones_Codigo",
                table: "Delegaciones",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Delegaciones_ParentId",
                table: "Delegaciones",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionesTemporales_DelegadoId_Estado",
                table: "DelegacionesTemporales",
                columns: new[] { "DelegadoId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionesTemporales_DeleganteId_Estado",
                table: "DelegacionesTemporales",
                columns: new[] { "DeleganteId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionRoles_RolId",
                table: "DelegacionRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionTemporalRoles_RolId",
                table: "DelegacionTemporalRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenciaModulos_ModuloId",
                table: "LicenciaModulos",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Licencias_Codigo",
                table: "Licencias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_Codigo",
                table: "Modulos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Codigo",
                table: "Permisos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_ModuloId",
                table: "Permisos",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ClearanceId",
                table: "Roles",
                column: "ClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_PermisoId",
                table: "RolPermisos",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeedVersions_Key",
                table: "SeedVersions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioContextos_ContextoId",
                table: "UsuarioContextos",
                column: "ContextoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioContextos_Primary",
                table: "UsuarioContextos",
                column: "UsuarioId",
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLicencias_LicenciaId",
                table: "UsuarioLicencias",
                column: "LicenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RolId",
                table: "UsuarioRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UserName",
                table: "Usuarios",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSesiones_UsuarioId_IsActive",
                table: "UsuarioSesiones",
                columns: new[] { "UsuarioId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContextoModulos");

            migrationBuilder.DropTable(
                name: "DelegacionRoles");

            migrationBuilder.DropTable(
                name: "DelegacionTemporalRoles");

            migrationBuilder.DropTable(
                name: "LicenciaModulos");

            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "SeedVersions");

            migrationBuilder.DropTable(
                name: "UsuarioContextos");

            migrationBuilder.DropTable(
                name: "UsuarioLicencias");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropTable(
                name: "UsuarioSesiones");

            migrationBuilder.DropTable(
                name: "DelegacionesTemporales");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Contextos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Delegaciones");

            migrationBuilder.DropTable(
                name: "Licencias");

            migrationBuilder.DropTable(
                name: "Clearances");
        }
    }
}
