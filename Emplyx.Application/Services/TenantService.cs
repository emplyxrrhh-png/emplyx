using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.Tenants;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Shared.Contracts.Tenants;

namespace Emplyx.Application.Services;

internal sealed class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TenantService(ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return tenants.Select(MapToResponse).ToList();
    }

    public async Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        return tenant is null ? null : MapToResponse(tenant);
    }

    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var internalId = string.IsNullOrWhiteSpace(request.InternalId) ? id.ToString() : request.InternalId;

        var tenant = new Tenant(
            id,
            internalId,
            request.CompanyType,
            request.LegalName,
            request.TaxId,
            MapToAddress(request.LegalAddress),
            MapToContactPerson(request.MainContact));

        // Update other fields using reflection or manual mapping if I add methods to Tenant
        // Since I didn't add a huge constructor or Setters, I need to use the UpdateDetails method or similar.
        // But I only added UpdateDetails for a few fields.
        // I should probably add a comprehensive Update method or use reflection for this demo, 
        // OR update the Tenant entity to have a comprehensive constructor or Setters (private set with methods).
        
        // For now, I'll assume I can add a method to Tenant to set all these optional fields.
        // Or I'll use reflection to set private properties (not recommended but fast).
        // Better: I'll update Tenant.cs to include a method `UpdateAll` or similar.
        
        // Let's update Tenant.cs first to allow setting all these properties.
        // But I can't edit Tenant.cs inside this tool call easily if I'm already writing Service.
        // I'll write the service assuming the method exists, then update Tenant.cs.
        
        UpdateTenantProperties(tenant, request);

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(tenant);
    }

    public async Task UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant is null) return;

        UpdateTenantProperties(tenant, request);

        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant is null) return;

        _tenantRepository.Remove(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTenantProperties(Tenant tenant, dynamic request)
    {
        tenant.UpdateFull(
            request.CompanyType,
            request.TradeName,
            request.CountryOfConstitution,
            request.DateOfConstitution,
            request.CommercialRegister,
            request.ProvinceOfRegister,
            request.RegisterDetails,
            MapToAddress(request.LegalAddress),
            request.FiscalAddress == null ? null : MapToAddress(request.FiscalAddress),
            MapToContactPerson(request.MainContact),
            request.GeneralPhone,
            request.GeneralEmail,
            request.BillingEmail,
            request.Website,
            request.SocialMedia,
            request.VATRegime,
            request.IntraCommunityVAT,
            request.IRPFRegime,
            request.Currency,
            request.PaymentMethod,
            request.PaymentTerm,
            request.CreditLimit,
            request.PORequired,
            request.InvoiceDeliveryMethod,
            request.BillingNotes,
            request.BankAccount == null ? null : MapToBankAccount(request.BankAccount),
            request.EmployeeCount,
            request.CollectiveAgreement,
            request.CNAE,
            request.WorkDayType,
            request.HasShifts,
            request.RelationType,
            request.Segment,
            request.Sector,
            request.CompanySize,
            request.AnnualRevenue,
            request.Source,
            request.InternalAccountManager,
            request.Status,
            request.PortalAccess,
            request.AdminUser == null ? null : MapToAdminUser(request.AdminUser),
            request.Language,
            request.TimeZone,
            request.CommercialComms,
            request.OperationalComms,
            request.PreferredChannel,
            request.GDPRConsent,
            request.TermsAccepted,
            request.PrivacyAccepted,
            request.AcceptanceDate,
            request.AcceptanceIP,
            request.InternalNotes,
            request.Tags
        );
    }

    private TenantResponse MapToResponse(Tenant t)
    {
        return new TenantResponse(
            t.Id, t.InternalId, t.CompanyType, t.LegalName, t.TradeName, t.TaxId,
            t.CountryOfConstitution, t.DateOfConstitution, t.CommercialRegister, t.ProvinceOfRegister, t.RegisterDetails,
            MapToAddressDto(t.LegalAddress), t.FiscalAddress == null ? null : MapToAddressDto(t.FiscalAddress),
            MapToContactPersonDto(t.MainContact),
            t.GeneralPhone, t.GeneralEmail, t.BillingEmail, t.Website, t.SocialMedia,
            t.VATRegime, t.IntraCommunityVAT, t.IRPFRegime, t.Currency, t.PaymentMethod, t.PaymentTerm,
            t.CreditLimit, t.PORequired, t.InvoiceDeliveryMethod, t.BillingNotes,
            t.BankAccount == null ? null : MapToBankAccountDto(t.BankAccount),
            t.EmployeeCount, t.CollectiveAgreement, t.CNAE, t.WorkDayType, t.HasShifts,
            t.RelationType, t.Segment, t.Sector, t.CompanySize, t.AnnualRevenue, t.Source, t.InternalAccountManager, t.Status,
            t.PortalAccess, t.AdminUser == null ? null : MapToAdminUserDto(t.AdminUser),
            t.Language, t.TimeZone, t.CommercialComms, t.OperationalComms, t.PreferredChannel,
            t.GDPRConsent, t.TermsAccepted, t.PrivacyAccepted, t.AcceptanceDate, t.AcceptanceIP,
            t.CreatedAtUtc, t.CreatedBy, t.InternalNotes, t.Tags
        );
    }

    private Address MapToAddress(AddressDto dto) => new(dto.Street, dto.ZipCode, dto.City, dto.Province, dto.Country);
    private AddressDto MapToAddressDto(Address a) => new(a.Street, a.ZipCode, a.City, a.Province, a.Country);
    
    private ContactPerson MapToContactPerson(ContactPersonDto dto) => new(dto.Name, dto.JobTitle, dto.Phone, dto.Mobile, dto.Email);
    private ContactPersonDto MapToContactPersonDto(ContactPerson c) => new(c.Name, c.JobTitle, c.Phone, c.Mobile, c.Email);

    private BankAccount MapToBankAccount(BankAccountDto dto) => new(dto.AccountHolder, dto.IBAN, dto.BIC, dto.BankName, dto.SEPAAuth, dto.SEPAAuthDate, dto.SEPAReference);
    private BankAccountDto MapToBankAccountDto(BankAccount b) => new(b.AccountHolder, b.IBAN, b.BIC, b.BankName, b.SEPAAuth, b.SEPAAuthDate, b.SEPAReference);

    private AdminUser MapToAdminUser(AdminUserDto dto) => new(dto.Name, dto.Email, dto.Phone);
    private AdminUserDto MapToAdminUserDto(AdminUser u) => new(u.Name, u.Email, u.Phone);
}
