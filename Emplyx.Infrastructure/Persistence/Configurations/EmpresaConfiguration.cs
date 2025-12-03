using Emplyx.Domain.Entities.Empresas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emplyx.Infrastructure.Persistence.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.InternalId).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CompanyType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.TradeName).HasMaxLength(200);
        builder.Property(e => e.TaxId).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CountryOfConstitution).HasMaxLength(100);
        builder.Property(e => e.CommercialRegister).HasMaxLength(200);
        builder.Property(e => e.ProvinceOfRegister).HasMaxLength(100);
        builder.Property(e => e.RegisterDetails).HasMaxLength(500);

        builder.OwnsOne(e => e.LegalAddress, a =>
        {
            a.Property(p => p.Street).HasMaxLength(200).HasColumnName("LegalAddress_Street");
            a.Property(p => p.ZipCode).HasMaxLength(20).HasColumnName("LegalAddress_ZipCode");
            a.Property(p => p.City).HasMaxLength(100).HasColumnName("LegalAddress_City");
            a.Property(p => p.Province).HasMaxLength(100).HasColumnName("LegalAddress_Province");
            a.Property(p => p.Country).HasMaxLength(100).HasColumnName("LegalAddress_Country");
        });

        builder.OwnsOne(e => e.FiscalAddress, a =>
        {
            a.Property(p => p.Street).HasMaxLength(200).HasColumnName("FiscalAddress_Street");
            a.Property(p => p.ZipCode).HasMaxLength(20).HasColumnName("FiscalAddress_ZipCode");
            a.Property(p => p.City).HasMaxLength(100).HasColumnName("FiscalAddress_City");
            a.Property(p => p.Province).HasMaxLength(100).HasColumnName("FiscalAddress_Province");
            a.Property(p => p.Country).HasMaxLength(100).HasColumnName("FiscalAddress_Country");
        });

        builder.OwnsOne(e => e.MainContact, c =>
        {
            c.Property(p => p.Name).HasMaxLength(100).HasColumnName("MainContact_Name");
            c.Property(p => p.JobTitle).HasMaxLength(100).HasColumnName("MainContact_JobTitle");
            c.Property(p => p.Phone).HasMaxLength(50).HasColumnName("MainContact_Phone");
            c.Property(p => p.Mobile).HasMaxLength(50).HasColumnName("MainContact_Mobile");
            c.Property(p => p.Email).HasMaxLength(100).HasColumnName("MainContact_Email");
        });

        builder.Property(e => e.GeneralPhone).HasMaxLength(50);
        builder.Property(e => e.GeneralEmail).HasMaxLength(100);
        builder.Property(e => e.BillingEmail).HasMaxLength(100);
        builder.Property(e => e.Website).HasMaxLength(200);
        builder.Property(e => e.SocialMedia).HasMaxLength(2000); // JSON

        builder.Property(e => e.VATRegime).HasMaxLength(50);
        builder.Property(e => e.IntraCommunityVAT).HasMaxLength(50);
        builder.Property(e => e.IRPFRegime).HasMaxLength(50);
        builder.Property(e => e.Currency).HasMaxLength(10);
        builder.Property(e => e.PaymentMethod).HasMaxLength(50);
        builder.Property(e => e.PaymentTerm).HasMaxLength(50);
        builder.Property(e => e.InvoiceDeliveryMethod).HasMaxLength(50);
        builder.Property(e => e.BillingNotes).HasMaxLength(1000);

        builder.OwnsOne(e => e.BankAccount, b =>
        {
            b.Property(p => p.AccountHolder).HasMaxLength(100).HasColumnName("BankAccount_AccountHolder");
            b.Property(p => p.IBAN).HasMaxLength(50).HasColumnName("BankAccount_IBAN");
            b.Property(p => p.BIC).HasMaxLength(20).HasColumnName("BankAccount_BIC");
            b.Property(p => p.BankName).HasMaxLength(100).HasColumnName("BankAccount_BankName");
            b.Property(p => p.SEPAReference).HasMaxLength(100).HasColumnName("BankAccount_SEPAReference");
        });

        builder.OwnsOne(e => e.AdminUser, u =>
        {
            u.Property(p => p.Name).HasMaxLength(100).HasColumnName("AdminUser_Name");
            u.Property(p => p.Email).HasMaxLength(100).HasColumnName("AdminUser_Email");
            u.Property(p => p.Phone).HasMaxLength(50).HasColumnName("AdminUser_Phone");
        });

        builder.Property(e => e.Language).HasMaxLength(10);
        builder.Property(e => e.TimeZone).HasMaxLength(50);
        builder.Property(e => e.InternalNotes).HasMaxLength(2000);
        builder.Property(e => e.Tags).HasMaxLength(500);

        builder.Property(e => e.IsActive).HasDefaultValue(true);
    }
}
