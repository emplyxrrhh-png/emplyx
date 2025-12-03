using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emplyx.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountryOfConstitution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfConstitution = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommercialRegister = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProvinceOfRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegisterDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalAddress_Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LegalAddress_ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LegalAddress_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalAddress_Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalAddress_Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FiscalAddress_Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FiscalAddress_ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FiscalAddress_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FiscalAddress_Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FiscalAddress_Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MainContact_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MainContact_JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MainContact_Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainContact_Mobile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainContact_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GeneralPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GeneralEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BillingEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SocialMedia = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VATRegime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IntraCommunityVAT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IRPFRegime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentTerm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PORequired = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceDeliveryMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BillingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BankAccount_AccountHolder = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    BankAccount_IBAN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankAccount_BIC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BankAccount_BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankAccount_SEPAAuth = table.Column<bool>(type: "bit", nullable: true),
                    BankAccount_SEPAAuthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankAccount_SEPAReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeCount = table.Column<int>(type: "int", nullable: true),
                    CollectiveAgreement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CNAE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkDayType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasShifts = table.Column<bool>(type: "bit", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Segment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanySize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnnualRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalAccountManager = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PortalAccess = table.Column<bool>(type: "bit", nullable: false),
                    AdminUser_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AdminUser_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdminUser_Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CommercialComms = table.Column<bool>(type: "bit", nullable: false),
                    OperationalComms = table.Column<bool>(type: "bit", nullable: false),
                    PreferredChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GDPRConsent = table.Column<bool>(type: "bit", nullable: false),
                    TermsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyAccepted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptanceIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
