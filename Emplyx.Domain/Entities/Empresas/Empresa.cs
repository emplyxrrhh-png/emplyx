using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Empresas;

public class Empresa : Entity, IAggregateRoot
{
    // Inheritance Flags
    public bool InheritAddresses { get; private set; }
    public bool InheritContact { get; private set; }
    public bool InheritFiscal { get; private set; }
    public bool InheritBank { get; private set; }
    public bool InheritAccess { get; private set; }

    // 1. Datos identificativos
    public Guid TenantId { get; private set; }

    public string InternalId { get; private set; }
    public string CompanyType { get; private set; }
    public string LegalName { get; private set; }
    public string? TradeName { get; private set; }
    public string TaxId { get; private set; }
    public string? CountryOfConstitution { get; private set; }
    public DateTime? DateOfConstitution { get; private set; }
    public string? CommercialRegister { get; private set; }
    public string? ProvinceOfRegister { get; private set; }
    public string? RegisterDetails { get; private set; }

    // 2. Direcciones
    public Address LegalAddress { get; private set; }
    public Address? FiscalAddress { get; private set; }
    
    // 3. Datos de contacto
    public ContactPerson MainContact { get; private set; }
    public string? GeneralPhone { get; private set; }
    public string? GeneralEmail { get; private set; }
    public string? BillingEmail { get; private set; }
    public string? Website { get; private set; }
    public string? SocialMedia { get; private set; }

    // 4. Datos fiscales y de facturación
    public string? VATRegime { get; private set; }
    public string? IntraCommunityVAT { get; private set; }
    public string? IRPFRegime { get; private set; }
    public string? Currency { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? PaymentTerm { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public bool PORequired { get; private set; }
    public string? InvoiceDeliveryMethod { get; private set; }
    public string? BillingNotes { get; private set; }

    // 5. Datos bancarios
    public BankAccount? BankAccount { get; private set; }

    // 8. Configuración de acceso
    public bool PortalAccess { get; private set; }
    public AdminUser? AdminUser { get; private set; }
    public string? Language { get; private set; }
    public string? TimeZone { get; private set; }

    // Control
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public string? InternalNotes { get; private set; }
    public string? Tags { get; private set; }

    private Empresa() 
    {
        InternalId = null!;
        CompanyType = null!;
        LegalName = null!;
        TaxId = null!;
        LegalAddress = null!;
        MainContact = null!;
    }

    public Empresa(
        Guid id,
        Guid tenantId,
        string internalId,
        string companyType,
        string legalName,
        string taxId,
        Address legalAddress,
        ContactPerson mainContact)
        : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id must be provided.", nameof(tenantId));
        }

        TenantId = tenantId;
        InternalId = internalId;
        CompanyType = companyType;
        LegalName = legalName;
        TaxId = taxId;
        LegalAddress = legalAddress;
        MainContact = mainContact;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        bool inheritAddresses, bool inheritContact, bool inheritFiscal, bool inheritBank, bool inheritAccess,
        string companyType, string? tradeName, string? countryOfConstitution, DateTime? dateOfConstitution,
        string? commercialRegister, string? provinceOfRegister, string? registerDetails,
        Address legalAddress, Address? fiscalAddress,
        ContactPerson mainContact, string? generalPhone, string? generalEmail, string? billingEmail, string? website, string? socialMedia,
        string? vatRegime, string? intraCommunityVAT, string? irpfRegime, string? currency, string? paymentMethod, string? paymentTerm, decimal? creditLimit, bool poRequired, string? invoiceDeliveryMethod, string? billingNotes,
        BankAccount? bankAccount,
        bool portalAccess, AdminUser? adminUser, string? language, string? timeZone,
        string? internalNotes, string? tags)
    {
        InheritAddresses = inheritAddresses;
        InheritContact = inheritContact;
        InheritFiscal = inheritFiscal;
        InheritBank = inheritBank;
        InheritAccess = inheritAccess;

        CompanyType = companyType;
        TradeName = tradeName;
        CountryOfConstitution = countryOfConstitution;
        DateOfConstitution = dateOfConstitution;
        CommercialRegister = commercialRegister;
        ProvinceOfRegister = provinceOfRegister;
        RegisterDetails = registerDetails;
        LegalAddress = legalAddress;
        FiscalAddress = fiscalAddress;
        MainContact = mainContact;
        GeneralPhone = generalPhone;
        GeneralEmail = generalEmail;
        BillingEmail = billingEmail;
        Website = website;
        SocialMedia = socialMedia;
        VATRegime = vatRegime;
        IntraCommunityVAT = intraCommunityVAT;
        IRPFRegime = irpfRegime;
        Currency = currency;
        PaymentMethod = paymentMethod;
        PaymentTerm = paymentTerm;
        CreditLimit = creditLimit;
        PORequired = poRequired;
        InvoiceDeliveryMethod = invoiceDeliveryMethod;
        BillingNotes = billingNotes;
        BankAccount = bankAccount;
        PortalAccess = portalAccess;
        AdminUser = adminUser;
        Language = language;
        TimeZone = timeZone;
        InternalNotes = internalNotes;
        Tags = tags;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AssignTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id must be provided.", nameof(tenantId));
        }

        TenantId = tenantId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public record Address(string Street, string ZipCode, string City, string Province, string Country);
public record ContactPerson(string Name, string JobTitle, string Phone, string Mobile, string Email);
public record BankAccount(string AccountHolder, string IBAN, string BIC, string BankName, bool SEPAAuth, DateTime? SEPAAuthDate, string? SEPAReference);
public record AdminUser(string Name, string Email, string Phone);
