using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emplyx.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SeedVersions",
                keyColumn: "Id",
                keyValue: new Guid("d6e36d75-6f90-4d90-b1b4-0d423b99d8f1"),
                column: "Version",
                value: 2);

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "ClearanceId", "CreatedAtUtc", "DisplayName", "Email", "ExternalIdentityId", "IsActive", "LastLoginAtUtc", "LastPasswordChangeAtUtc", "PasswordHash", "PreferredContextoId", "UpdatedAtUtc", "UserName", "PerfilApellidos", "PerfilCargo", "PerfilDepartamento", "PerfilNombres", "PerfilTelefono" },
                values: new object[] { new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), new Guid("5d7a9d25-c4b2-4b93-93c6-a39c47da1f40"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ezequiel Mizrahi", "emizrahi@emplyx.com", null, true, null, null, "P9cgwXEEYV832bo5ChDPcBiX1ZT6L6dTVWQRF5GG4gQ=", new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "emizrahi", "Mizrahi", "Admin", "IT", "Ezequiel", null });

            migrationBuilder.InsertData(
                table: "UsuarioContextos",
                columns: new[] { "ContextoId", "UsuarioId", "IsPrimary", "LinkedAtUtc" },
                values: new object[] { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "UsuarioLicencias",
                columns: new[] { "LicenciaId", "UsuarioId", "AssignedAtUtc" },
                values: new object[] { new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "UsuarioRoles",
                columns: new[] { "ContextoId", "RolId", "UsuarioId", "AssignedAtUtc" },
                values: new object[] { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsuarioContextos",
                keyColumns: new[] { "ContextoId", "UsuarioId" },
                keyValues: new object[] { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2") });

            migrationBuilder.DeleteData(
                table: "UsuarioLicencias",
                keyColumns: new[] { "LicenciaId", "UsuarioId" },
                keyValues: new object[] { new Guid("b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2") });

            migrationBuilder.DeleteData(
                table: "UsuarioRoles",
                keyColumns: new[] { "ContextoId", "RolId", "UsuarioId" },
                keyValues: new object[] { new Guid("caa6d616-f237-4d2b-a3f1-bf54cd508c35"), new Guid("3fbdede0-0773-4cb6-9881-9350d0f4955e"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2") });

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"));

            migrationBuilder.UpdateData(
                table: "SeedVersions",
                keyColumn: "Id",
                keyValue: new Guid("d6e36d75-6f90-4d90-b1b4-0d423b99d8f1"),
                column: "Version",
                value: 1);
        }
    }
}
