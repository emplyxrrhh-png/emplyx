namespace Emplyx.Shared.Contracts.Empresas;

public record CreateEmpresaRequest(
    string Nombre,
    string RazonSocial,
    string CIF,
    string? Direccion,
    string? Telefono,
    string? Email,
    string? Web,
    string? Pais);
