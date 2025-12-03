using Emplyx.Application.Abstractions;
using Emplyx.Domain.Entities.Empresas;
using Emplyx.Domain.Repositories;
using Emplyx.Domain.UnitOfWork;
using Emplyx.Shared.Contracts.Empresas;

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
        return empresas.Select(e => new EmpresaResponse(
            e.Id,
            e.Nombre,
            e.RazonSocial,
            e.CIF,
            e.Direccion,
            e.Telefono,
            e.Email,
            e.Web,
            e.Pais,
            e.IsActive)).ToList();
    }

    public async Task<EmpresaResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken);
        if (empresa is null)
        {
            return null;
        }

        return new EmpresaResponse(
            empresa.Id,
            empresa.Nombre,
            empresa.RazonSocial,
            empresa.CIF,
            empresa.Direccion,
            empresa.Telefono,
            empresa.Email,
            empresa.Web,
            empresa.Pais,
            empresa.IsActive);
    }

    public async Task<EmpresaResponse> CreateAsync(CreateEmpresaRequest request, CancellationToken cancellationToken = default)
    {
        var empresa = new Empresa(
            Guid.NewGuid(),
            request.Nombre,
            request.RazonSocial,
            request.CIF,
            request.Direccion,
            request.Telefono,
            request.Email,
            request.Web,
            request.Pais);

        await _empresaRepository.AddAsync(empresa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EmpresaResponse(
            empresa.Id,
            empresa.Nombre,
            empresa.RazonSocial,
            empresa.CIF,
            empresa.Direccion,
            empresa.Telefono,
            empresa.Email,
            empresa.Web,
            empresa.Pais,
            empresa.IsActive);
    }

    public async Task UpdateAsync(Guid id, UpdateEmpresaRequest request, CancellationToken cancellationToken = default)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken);
        if (empresa is null)
        {
            // Handle not found
            return;
        }

        empresa.Update(
            request.Nombre,
            request.RazonSocial,
            request.CIF,
            request.Direccion,
            request.Telefono,
            request.Email,
            request.Web,
            request.Pais);

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
}
