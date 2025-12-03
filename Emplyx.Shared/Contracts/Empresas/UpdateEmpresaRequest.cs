namespace Emplyx.Shared.Contracts.Empresas;

public record UpdateEmpresaRequest(
    string Nombre,
    string RazonSocial,
    string CIF,
    string? Direccion,
    string? Telefono,
    string? Email,
    string? Web,
    string? Pais);
