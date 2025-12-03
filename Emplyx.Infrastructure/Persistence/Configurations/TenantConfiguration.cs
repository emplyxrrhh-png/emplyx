using Emplyx.Domain.Entities.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.InternalId).HasMaxLength(50).IsRequired();
        builder.Property(t => t.CompanyType).HasMaxLength(50);
        builder.Property(t => t.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(t => t.TradeName).HasMaxLength(200);
        builder.Property(t => t.TaxId).HasMaxLength(50).IsRequired();
        builder.Property(t => t.CountryOfConstitution).HasMaxLength(100);
        builder.Property(t => t.CommercialRegister).HasMaxLength(200);
        builder.Property(t => t.ProvinceOfRegister).HasMaxLength(100);
        builder.Property(t => t.RegisterDetails).HasMaxLength(500);

        builder.OwnsOne(t => t.LegalAddress, a =>
        {
            a.Property(p => p.Street).HasMaxLength(200).HasColumnName("LegalAddress_Street");
            a.Property(p => p.ZipCode).HasMaxLength(20).HasColumnName("LegalAddress_ZipCode");
            a.Property(p => p.City).HasMaxLength(100).HasColumnName("LegalAddress_City");
            a.Property(p => p.Province).HasMaxLength(100).HasColumnName("LegalAddress_Province");
            a.Property(p => p.Country).HasMaxLength(100).HasColumnName("LegalAddress_Country");
        });

        builder.OwnsOne(t => t.FiscalAddress, a =>
        {
            a.Property(p => p.Street).HasMaxLength(200).HasColumnName("FiscalAddress_Street");
            a.Property(p => p.ZipCode).HasMaxLength(20).HasColumnName("FiscalAddress_ZipCode");
            a.Property(p => p.City).HasMaxLength(100).HasColumnName("FiscalAddress_City");
            a.Property(p => p.Province).HasMaxLength(100).HasColumnName("FiscalAddress_Province");
            a.Property(p => p.Country).HasMaxLength(100).HasColumnName("FiscalAddress_Country");
        });

        builder.OwnsOne(t => t.MainContact, c =>
        {
            c.Property(p => p.Name).HasMaxLength(150).HasColumnName("MainContact_Name");
            c.Property(p => p.JobTitle).HasMaxLength(100).HasColumnName("MainContact_JobTitle");
            c.Property(p => p.Phone).HasMaxLength(50).HasColumnName("MainContact_Phone");
            c.Property(p => p.Mobile).HasMaxLength(50).HasColumnName("MainContact_Mobile");
            c.Property(p => p.Email).HasMaxLength(100).HasColumnName("MainContact_Email");
        });

        builder.Property(t => t.GeneralPhone).HasMaxLength(50);
        builder.Property(t => t.GeneralEmail).HasMaxLength(100);
        builder.Property(t => t.BillingEmail).HasMaxLength(100);
        builder.Property(t => t.Website).HasMaxLength(200);
        builder.Property(t => t.SocialMedia).HasMaxLength(1000);

        builder.Property(t => t.VATRegime).HasMaxLength(100);
        builder.Property(t => t.IntraCommunityVAT).HasMaxLength(50);
        builder.Property(t => t.IRPFRegime).HasMaxLength(100);
        builder.Property(t => t.Currency).HasMaxLength(10);
        builder.Property(t => t.PaymentMethod).HasMaxLength(100);
        builder.Property(t => t.PaymentTerm).HasMaxLength(100);
        builder.Property(t => t.CreditLimit).HasColumnType("decimal(18,2)");
        builder.Property(t => t.InvoiceDeliveryMethod).HasMaxLength(50);
        builder.Property(t => t.BillingNotes).HasMaxLength(1000);

        builder.OwnsOne(t => t.BankAccount, b =>
        {
            b.Property(p => p.AccountHolder).HasMaxLength(150).HasColumnName("BankAccount_AccountHolder");
            b.Property(p => p.IBAN).HasMaxLength(50).HasColumnName("BankAccount_IBAN");
            b.Property(p => p.BIC).HasMaxLength(20).HasColumnName("BankAccount_BIC");
            b.Property(p => p.BankName).HasMaxLength(100).HasColumnName("BankAccount_BankName");
            b.Property(p => p.SEPAReference).HasMaxLength(100).HasColumnName("BankAccount_SEPAReference");
        });

        builder.Property(t => t.CollectiveAgreement).HasMaxLength(200);
        builder.Property(t => t.CNAE).HasMaxLength(50);
        builder.Property(t => t.WorkDayType).HasMaxLength(50);

        builder.Property(t => t.RelationType).HasMaxLength(50);
        builder.Property(t => t.Segment).HasMaxLength(50);
        builder.Property(t => t.Sector).HasMaxLength(100);
        builder.Property(t => t.CompanySize).HasMaxLength(50);
        builder.Property(t => t.AnnualRevenue).HasColumnType("decimal(18,2)");
        builder.Property(t => t.Source).HasMaxLength(100);
        builder.Property(t => t.InternalAccountManager).HasMaxLength(150);
        builder.Property(t => t.Status).HasMaxLength(50);

        builder.OwnsOne(t => t.AdminUser, u =>
        {
            u.Property(p => p.Name).HasMaxLength(150).HasColumnName("AdminUser_Name");
            u.Property(p => p.Email).HasMaxLength(100).HasColumnName("AdminUser_Email");
            u.Property(p => p.Phone).HasMaxLength(50).HasColumnName("AdminUser_Phone");
        });

        builder.Property(t => t.Language).HasMaxLength(10);
        builder.Property(t => t.TimeZone).HasMaxLength(50);
        builder.Property(t => t.PreferredChannel).HasMaxLength(50);
        builder.Property(t => t.AcceptanceIP).HasMaxLength(50);
        builder.Property(t => t.CreatedBy).HasMaxLength(100);
        builder.Property(t => t.InternalNotes).HasMaxLength(2000);
        builder.Property(t => t.Tags).HasMaxLength(500);
    }
}
