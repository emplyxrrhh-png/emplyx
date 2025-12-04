using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.CentrosTrabajo;

public sealed class CentroTrabajo : Entity, IAggregateRoot
{
    private CentroTrabajo() 
    {
        Address = null!;
        Contact = null!;
    }

    public CentroTrabajo(
        Guid id,
        string nombre,
        Guid empresaId,
        string internalId,
        Address address,
        Contact contact,
        string timeZone,
        string language)
        : base(id)
    {
        Nombre = nombre;
        EmpresaId = empresaId;
        InternalId = internalId;
        Address = address;
        Contact = contact;
        TimeZone = timeZone;
        Language = language;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Nombre { get; private set; } = string.Empty;
    public Guid EmpresaId { get; private set; }
    public string InternalId { get; private set; } = string.Empty;
    
    public Address Address { get; private set; }
    public Contact Contact { get; private set; }
    
    public string TimeZone { get; private set; } = string.Empty;
    public string Language { get; private set; } = string.Empty;
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(
        string nombre,
        string internalId,
        Address address,
        Contact contact,
        string timeZone,
        string language,
        bool isActive)
    {
        Nombre = nombre;
        InternalId = internalId;
        Address = address;
        Contact = contact;
        TimeZone = timeZone;
        Language = language;
        IsActive = isActive;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public record Address(string Street, string ZipCode, string City, string Province, string Country);
public record Contact(string Name, string Phone, string Email);
