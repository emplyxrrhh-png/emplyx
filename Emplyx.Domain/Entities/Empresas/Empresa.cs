using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Empresas;

public class Empresa : Entity, IAggregateRoot
{
    public string Nombre { get; private set; }
    public string RazonSocial { get; private set; }
    public string CIF { get; private set; } // VAT
    public string? Direccion { get; private set; }
    public string? Telefono { get; private set; }
    public string? Email { get; private set; }
    public string? Web { get; private set; }
    public string? Pais { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private Empresa() 
    {
        Nombre = null!;
        RazonSocial = null!;
        CIF = null!;
    } // For EF Core

    public Empresa(
        Guid id,
        string nombre,
        string razonSocial,
        string cif,
        string? direccion,
        string? telefono,
        string? email,
        string? web,
        string? pais)
        : base(id)
    {
        Nombre = nombre;
        RazonSocial = razonSocial;
        CIF = cif;
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
        Web = web;
        Pais = pais;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        string nombre,
        string razonSocial,
        string cif,
        string? direccion,
        string? telefono,
        string? email,
        string? web,
        string? pais)
    {
        Nombre = nombre;
        RazonSocial = razonSocial;
        CIF = cif;
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
        Web = web;
        Pais = pais;
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
}
