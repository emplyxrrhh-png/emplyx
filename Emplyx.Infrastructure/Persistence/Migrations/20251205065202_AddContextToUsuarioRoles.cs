using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emplyx.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContextToUsuarioRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRoles",
                table: "UsuarioRoles");

            migrationBuilder.AddColumn<Guid>(
                name: "ContextoId",
                table: "UsuarioRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRoles",
                table: "UsuarioRoles",
                columns: new[] { "UsuarioId", "RolId", "ContextoId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRoles",
                table: "UsuarioRoles");

            migrationBuilder.DropColumn(
                name: "ContextoId",
                table: "UsuarioRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRoles",
                table: "UsuarioRoles",
                columns: new[] { "UsuarioId", "RolId" });
        }
    }
}
