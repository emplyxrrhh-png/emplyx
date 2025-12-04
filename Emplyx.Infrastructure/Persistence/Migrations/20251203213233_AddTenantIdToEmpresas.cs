using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emplyx.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add column as nullable first
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Empresas",
                type: "uniqueidentifier",
                nullable: true);

            // 2. Assign a valid TenantId to existing rows (Pick the first available tenant)
            migrationBuilder.Sql("UPDATE Empresas SET TenantId = (SELECT TOP 1 Id FROM Tenants)");

            // 3. Make the column non-nullable
            // Note: This will fail if there are no tenants. Ensure at least one tenant exists.
            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Empresas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"), // Default for new rows if not specified
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_TenantId",
                table: "Empresas",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_Tenants_TenantId",
                table: "Empresas",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_Tenants_TenantId",
                table: "Empresas");

            migrationBuilder.DropIndex(
                name: "IX_Empresas_TenantId",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Empresas");
        }
    }
}
