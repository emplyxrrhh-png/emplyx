using System;
using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.Empresas;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Shared.Contracts.Empresas;
using Emplyx.Shared.Contracts.Tenants;

namespace Emplyx.Application.Services;

internal sealed class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmpresaService(IEmpresaRepository empresaRepository, IUnitOfWork unitOfWork)
    {
        _empresaRepository = empresaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EmpresaResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var empresas = await _empresaRepository.GetAllAsync(cancellationToken);
        return empresas.Select(MapToResponse).ToList();
    }

    public async Task<List<EmpresaResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId is required", nameof(tenantId));
        }

        var empresas = await _empresaRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        return empresas.Select(MapToResponse).ToList();
    }

    public async Task<EmpresaResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken);
        return empresa is null ? null : MapToResponse(empresa);
    }

    public async Task<EmpresaResponse> CreateAsync(CreateEmpresaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId is required", nameof(request.TenantId));
        }

        var empresa = new Empresa(
            Guid.NewGuid(),
            request.TenantId,
            request.InternalId,
            request.CompanyType,
            request.LegalName,
            request.TaxId,
            MapAddress(request.LegalAddress),
            MapContact(request.MainContact));

        // Apply full update for optional fields
        empresa.Update(
            request.InheritAddresses, request.InheritContact, request.InheritFiscal, request.InheritBank, request.InheritAccess,
            request.CompanyType, request.TradeName, request.CountryOfConstitution, request.DateOfConstitution,
            request.CommercialRegister, request.ProvinceOfRegister, request.RegisterDetails,
            MapAddress(request.LegalAddress), request.FiscalAddress != null ? MapAddress(request.FiscalAddress) : null,
            MapContact(request.MainContact), request.GeneralPhone, request.GeneralEmail, request.BillingEmail, request.Website, request.SocialMedia,
            request.VATRegime, request.IntraCommunityVAT, request.IRPFRegime, request.Currency, request.PaymentMethod, request.PaymentTerm, request.CreditLimit, request.PORequired, request.InvoiceDeliveryMethod, request.BillingNotes,
            request.BankAccount != null ? MapBankAccount(request.BankAccount) : null,
            request.PortalAccess, request.AdminUser != null ? MapAdminUser(request.AdminUser) : null, request.Language, request.TimeZone,
            request.InternalNotes, request.Tags);

        await _empresaRepository.AddAsync(empresa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(empresa);
    }

    public async Task UpdateAsync(Guid id, UpdateEmpresaRequest request, CancellationToken cancellationToken = default)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken);
        if (empresa is null)
        {
            return;
        }

        empresa.Update(
            request.InheritAddresses, request.InheritContact, request.InheritFiscal, request.InheritBank, request.InheritAccess,
            request.CompanyType, request.TradeName, request.CountryOfConstitution, request.DateOfConstitution,
            request.CommercialRegister, request.ProvinceOfRegister, request.RegisterDetails,
            MapAddress(request.LegalAddress), request.FiscalAddress != null ? MapAddress(request.FiscalAddress) : null,
            MapContact(request.MainContact), request.GeneralPhone, request.GeneralEmail, request.BillingEmail, request.Website, request.SocialMedia,
            request.VATRegime, request.IntraCommunityVAT, request.IRPFRegime, request.Currency, request.PaymentMethod, request.PaymentTerm, request.CreditLimit, request.PORequired, request.InvoiceDeliveryMethod, request.BillingNotes,
            request.BankAccount != null ? MapBankAccount(request.BankAccount) : null,
            request.PortalAccess, request.AdminUser != null ? MapAdminUser(request.AdminUser) : null, request.Language, request.TimeZone,
            request.InternalNotes, request.Tags);

        _empresaRepository.Update(empresa);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken);
        if (empresa is null)
        {
            return;
        }

        _empresaRepository.Remove(empresa);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static EmpresaResponse MapToResponse(Empresa e)
    {
        return new EmpresaResponse(
            e.Id,
            e.TenantId,
            e.InheritAddresses,
            e.InheritContact,
            e.InheritFiscal,
            e.InheritBank,
            e.InheritAccess,
            e.InternalId,
            e.CompanyType,
            e.LegalName,
            e.TradeName,
            e.TaxId,
            e.CountryOfConstitution,
            e.DateOfConstitution,
            e.CommercialRegister,
            e.ProvinceOfRegister,
            e.RegisterDetails,
            new AddressDto(e.LegalAddress.Street, e.LegalAddress.ZipCode, e.LegalAddress.City, e.LegalAddress.Province, e.LegalAddress.Country),
            e.FiscalAddress != null ? new AddressDto(e.FiscalAddress.Street, e.FiscalAddress.ZipCode, e.FiscalAddress.City, e.FiscalAddress.Province, e.FiscalAddress.Country) : null,
            new ContactPersonDto(e.MainContact.Name, e.MainContact.JobTitle, e.MainContact.Phone, e.MainContact.Mobile, e.MainContact.Email),
            e.GeneralPhone,
            e.GeneralEmail,
            e.BillingEmail,
            e.Website,
            e.SocialMedia,
            e.VATRegime,
            e.IntraCommunityVAT,
            e.IRPFRegime,
            e.Currency,
            e.PaymentMethod,
            e.PaymentTerm,
            e.CreditLimit,
            e.PORequired,
            e.InvoiceDeliveryMethod,
            e.BillingNotes,
            e.BankAccount != null ? new BankAccountDto(e.BankAccount.AccountHolder, e.BankAccount.IBAN, e.BankAccount.BIC, e.BankAccount.BankName, e.BankAccount.SEPAAuth, e.BankAccount.SEPAAuthDate, e.BankAccount.SEPAReference) : null,
            e.PortalAccess,
            e.AdminUser != null ? new AdminUserDto(e.AdminUser.Name, e.AdminUser.Email, e.AdminUser.Phone) : null,
            e.Language,
            e.TimeZone,
            e.IsActive,
            e.InternalNotes,
            e.Tags
        );
    }

    private static Address MapAddress(AddressDto dto) => new Address(dto.Street, dto.ZipCode, dto.City, dto.Province, dto.Country);
    private static ContactPerson MapContact(ContactPersonDto dto) => new ContactPerson(dto.Name, dto.JobTitle, dto.Phone, dto.Mobile, dto.Email);
    private static BankAccount MapBankAccount(BankAccountDto dto) => new BankAccount(dto.AccountHolder, dto.IBAN, dto.BIC, dto.BankName, dto.SEPAAuth, dto.SEPAAuthDate, dto.SEPAReference);
    private static AdminUser MapAdminUser(AdminUserDto dto) => new AdminUser(dto.Name, dto.Email, dto.Phone);
}
