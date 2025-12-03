namespace Emplyx.Shared.Contracts.Empresas;

public record EmpresaResponse(
    Guid Id,
    string Nombre,
    string RazonSocial,
    string CIF,
    string? Direccion,
    string? Telefono,
    string? Email,
    string? Web,
    string? Pais,
    bool IsActive);
