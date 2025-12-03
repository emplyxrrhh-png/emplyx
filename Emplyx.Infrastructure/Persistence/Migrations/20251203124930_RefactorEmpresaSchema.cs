using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emplyx.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEmpresaSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Empresas",
                newName: "MainContact_Email");

            migrationBuilder.RenameColumn(
                name: "Web",
                table: "Empresas",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Empresas",
                newName: "VATRegime");

            migrationBuilder.RenameColumn(
                name: "RazonSocial",
                table: "Empresas",
                newName: "LegalName");

            migrationBuilder.RenameColumn(
                name: "Pais",
                table: "Empresas",
                newName: "ProvinceOfRegister");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Empresas",
                newName: "LegalAddress_Street");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Empresas",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "CIF",
                table: "Empresas",
                newName: "TaxId");

            migrationBuilder.AlterColumn<string>(
                name: "MainContact_Email",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminUser_Email",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminUser_Name",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminUser_Phone",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_AccountHolder",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_BIC",
                table: "Empresas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_BankName",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_IBAN",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BankAccount_SEPAAuth",
                table: "Empresas",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BankAccount_SEPAAuthDate",
                table: "Empresas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_SEPAReference",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingEmail",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingNotes",
                table: "Empresas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegister",
                table: "Empresas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryOfConstitution",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "Empresas",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Empresas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfConstitution",
                table: "Empresas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalAddress_City",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalAddress_Country",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalAddress_Province",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalAddress_Street",
                table: "Empresas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalAddress_ZipCode",
                table: "Empresas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralEmail",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralPhone",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IRPFRegime",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InheritAccess",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InheritAddresses",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InheritBank",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InheritContact",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InheritFiscal",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InternalId",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "Empresas",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntraCommunityVAT",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceDeliveryMethod",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Empresas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_City",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_Country",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_Province",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_ZipCode",
                table: "Empresas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainContact_JobTitle",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainContact_Mobile",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainContact_Name",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainContact_Phone",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PORequired",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerm",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PortalAccess",
                table: "Empresas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RegisterDetails",
                table: "Empresas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMedia",
                table: "Empresas",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Empresas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeName",
                table: "Empresas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUser_Email",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "AdminUser_Name",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "AdminUser_Phone",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_AccountHolder",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_BIC",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_BankName",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_IBAN",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_SEPAAuth",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_SEPAAuthDate",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BankAccount_SEPAReference",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BillingEmail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "BillingNotes",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "CommercialRegister",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "CountryOfConstitution",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "DateOfConstitution",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "FiscalAddress_City",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "FiscalAddress_Country",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "FiscalAddress_Province",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "FiscalAddress_Street",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "FiscalAddress_ZipCode",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "GeneralEmail",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "GeneralPhone",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "IRPFRegime",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InheritAccess",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InheritAddresses",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InheritBank",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InheritContact",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InheritFiscal",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InternalId",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "IntraCommunityVAT",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "InvoiceDeliveryMethod",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "LegalAddress_City",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "LegalAddress_Country",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "LegalAddress_Province",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "LegalAddress_ZipCode",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "MainContact_JobTitle",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "MainContact_Mobile",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "MainContact_Name",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "MainContact_Phone",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "PORequired",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "PaymentTerm",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "PortalAccess",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "RegisterDetails",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "SocialMedia",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "TradeName",
                table: "Empresas");

            migrationBuilder.RenameColumn(
                name: "MainContact_Email",
                table: "Empresas",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "Empresas",
                newName: "Web");

            migrationBuilder.RenameColumn(
                name: "VATRegime",
                table: "Empresas",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "TaxId",
                table: "Empresas",
                newName: "CIF");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "Empresas",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "ProvinceOfRegister",
                table: "Empresas",
                newName: "Pais");

            migrationBuilder.RenameColumn(
                name: "LegalName",
                table: "Empresas",
                newName: "RazonSocial");

            migrationBuilder.RenameColumn(
                name: "LegalAddress_Street",
                table: "Empresas",
                newName: "Nombre");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Empresas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
