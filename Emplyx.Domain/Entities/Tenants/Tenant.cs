using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Tenants;

public class Tenant : Entity, IAggregateRoot
{
    // 1. Datos identificativos
    public string InternalId { get; private set; }
    public string CompanyType { get; private set; } // SL, SA, etc.
    public string LegalName { get; private set; }
    public string? TradeName { get; private set; }
    public string TaxId { get; private set; } // NIF/CIF
    public string? CountryOfConstitution { get; private set; }
    public DateTime? DateOfConstitution { get; private set; }
    public string? CommercialRegister { get; private set; }
    public string? ProvinceOfRegister { get; private set; }
    public string? RegisterDetails { get; private set; } // Tomo, Libro, etc.

    // 2. Direcciones (Owned Types in EF Core usually, but flattened here for simplicity in this step or separate properties)
    public Address LegalAddress { get; private set; }
    public Address? FiscalAddress { get; private set; }
    
    // 3. Datos de contacto
    public ContactPerson MainContact { get; private set; }
    public string? GeneralPhone { get; private set; }
    public string? GeneralEmail { get; private set; }
    public string? BillingEmail { get; private set; }
    public string? Website { get; private set; }
    public string? SocialMedia { get; private set; } // JSON string

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

    // 6. Datos laborales / RRHH
    public int? EmployeeCount { get; private set; }
    public string? CollectiveAgreement { get; private set; }
    public string? CNAE { get; private set; }
    public string? WorkDayType { get; private set; }
    public bool HasShifts { get; private set; }
    
    // 7. Datos comerciales
    public string? RelationType { get; private set; }
    public string? Segment { get; private set; }
    public string? Sector { get; private set; }
    public string? CompanySize { get; private set; }
    public decimal? AnnualRevenue { get; private set; }
    public string? Source { get; private set; }
    public string? InternalAccountManager { get; private set; }
    public string? Status { get; private set; }

    // 8. Configuración de acceso
    public bool PortalAccess { get; private set; }
    public AdminUser? AdminUser { get; private set; }
    public string? Language { get; private set; }
    public string? TimeZone { get; private set; }

    // 9. Preferencias y consentimientos
    public bool CommercialComms { get; private set; }
    public bool OperationalComms { get; private set; }
    public string? PreferredChannel { get; private set; }
    public bool GDPRConsent { get; private set; }
    public bool TermsAccepted { get; private set; }
    public bool PrivacyAccepted { get; private set; }
    public DateTime? AcceptanceDate { get; private set; }
    public string? AcceptanceIP { get; private set; }

    // 11. Campos de control internos
    public DateTime CreatedAtUtc { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? InternalNotes { get; private set; }
    public string? Tags { get; private set; }

    private Tenant() 
    {
        InternalId = null!;
        CompanyType = null!;
        LegalName = null!;
        TaxId = null!;
        LegalAddress = null!;
        MainContact = null!;
    }

    public Tenant(
        Guid id,
        string internalId,
        string companyType,
        string legalName,
        string taxId,
        Address legalAddress,
        ContactPerson mainContact) : base(id)
    {
        InternalId = internalId;
        CompanyType = companyType;
        LegalName = legalName;
        TaxId = taxId;
        LegalAddress = legalAddress;
        MainContact = mainContact;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateFull(
        string companyType, string? tradeName, string? countryOfConstitution, DateTime? dateOfConstitution,
        string? commercialRegister, string? provinceOfRegister, string? registerDetails,
        Address legalAddress, Address? fiscalAddress,
        ContactPerson mainContact, string? generalPhone, string? generalEmail, string? billingEmail, string? website, string? socialMedia,
        string? vatRegime, string? intraCommunityVAT, string? irpfRegime, string? currency, string? paymentMethod, string? paymentTerm, decimal? creditLimit, bool poRequired, string? invoiceDeliveryMethod, string? billingNotes,
        BankAccount? bankAccount,
        int? employeeCount, string? collectiveAgreement, string? cnae, string? workDayType, bool hasShifts,
        string? relationType, string? segment, string? sector, string? companySize, decimal? annualRevenue, string? source, string? internalAccountManager, string? status,
        bool portalAccess, AdminUser? adminUser, string? language, string? timeZone,
        bool commercialComms, bool operationalComms, string? preferredChannel, bool gdprConsent, bool termsAccepted, bool privacyAccepted, DateTime? acceptanceDate, string? acceptanceIP,
        string? internalNotes, string? tags)
    {
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
        EmployeeCount = employeeCount;
        CollectiveAgreement = collectiveAgreement;
        CNAE = cnae;
        WorkDayType = workDayType;
        HasShifts = hasShifts;
        RelationType = relationType;
        Segment = segment;
        Sector = sector;
        CompanySize = companySize;
        AnnualRevenue = annualRevenue;
        Source = source;
        InternalAccountManager = internalAccountManager;
        Status = status;
        PortalAccess = portalAccess;
        AdminUser = adminUser;
        Language = language;
        TimeZone = timeZone;
        CommercialComms = commercialComms;
        OperationalComms = operationalComms;
        PreferredChannel = preferredChannel;
        GDPRConsent = gdprConsent;
        TermsAccepted = termsAccepted;
        PrivacyAccepted = privacyAccepted;
        AcceptanceDate = acceptanceDate;
        AcceptanceIP = acceptanceIP;
        InternalNotes = internalNotes;
        Tags = tags;
        // UpdatedAtUtc = DateTime.UtcNow; // If I had this field
    }
}

public record Address(string Street, string ZipCode, string City, string Province, string Country);
public record ContactPerson(string Name, string JobTitle, string Phone, string Mobile, string Email);
public record BankAccount(string AccountHolder, string IBAN, string BIC, string BankName, bool SEPAAuth, DateTime? SEPAAuthDate, string? SEPAReference);
public record AdminUser(string Name, string Email, string Phone);
