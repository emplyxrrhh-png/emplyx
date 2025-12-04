using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Employees;

public sealed class Employee : Entity, IAggregateRoot
{
    private readonly List<EmployeeUserField> _userFields = new();

    private Employee() 
    {
        Nombre = null!;
        Apellidos = null!;
        Alias = null!;
        GroupName = null!;
        Type = null!;
        Status = null!;
    }

    public Employee(
        Guid id,
        Guid empresaId,
        Guid? centroTrabajoId,
        string nombre,
        string apellidos,
        string alias,
        string groupName,
        string type,
        string status,
        string? image = null)
        : base(id)
    {
        EmpresaId = empresaId;
        CentroTrabajoId = centroTrabajoId;
        Nombre = nombre;
        Apellidos = apellidos;
        Alias = alias;
        GroupName = groupName;
        Type = type;
        Status = status;
        Image = image;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid EmpresaId { get; private set; }
    public Guid? CentroTrabajoId { get; private set; }
    
    public string Nombre { get; private set; }
    public string Apellidos { get; private set; }
    public string Alias { get; private set; }
    public string GroupName { get; private set; }
    public string Type { get; private set; }
    public string Status { get; private set; }
    public string? Image { get; private set; }

    // Extended General Data
    public string? ContractType { get; private set; }
    public DateTime? StartDate { get; private set; }
    public string? Idioma { get; private set; }
    public bool RemoteWork { get; private set; }
    public string? Notes { get; private set; }

    // Control & Access
    public string? IDAccessGroup { get; private set; }
    public string? BiometricID { get; private set; }
    public bool AttControlled { get; private set; }
    public bool AccControlled { get; private set; }
    public bool JobControlled { get; private set; }
    public bool ExtControlled { get; private set; }
    public bool RiskControlled { get; private set; }

    // Web Credentials
    public string? WebLogin { get; private set; }
    public string? WebPassword { get; private set; }
    public bool ActiveDirectory { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<EmployeeUserField> UserFields => _userFields.AsReadOnly();

    public void UpdateDetails(
        string nombre, 
        string apellidos,
        string alias, 
        string groupName, 
        string type, 
        string status, 
        string? image,
        string? contractType,
        DateTime? startDate,
        string? idioma,
        bool remoteWork,
        string? notes)
    {
        Nombre = nombre;
        Apellidos = apellidos;
        Alias = alias;
        GroupName = groupName;
        Type = type;
        Status = status;
        Image = image;
        ContractType = contractType;
        StartDate = startDate;
        Idioma = idioma;
        RemoteWork = remoteWork;
        Notes = notes;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateControl(string? idAccessGroup, string? biometricID, bool att, bool acc, bool job, bool ext, bool risk)
    {
        IDAccessGroup = idAccessGroup;
        BiometricID = biometricID;
        AttControlled = att;
        AccControlled = acc;
        JobControlled = job;
        ExtControlled = ext;
        RiskControlled = risk;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateCredentials(string? webLogin, string? webPassword, bool activeDirectory)
    {
        WebLogin = webLogin;
        WebPassword = webPassword;
        ActiveDirectory = activeDirectory;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void MoveTo(Guid? centroTrabajoId, Guid empresaId)
    {
        CentroTrabajoId = centroTrabajoId;
        EmpresaId = empresaId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetUserField(string fieldDefinitionId, string value)
    {
        var existing = _userFields.FirstOrDefault(f => f.FieldDefinitionId == fieldDefinitionId);
        if (existing != null)
        {
            existing.UpdateValue(value);
        }
        else
        {
            _userFields.Add(new EmployeeUserField(Guid.NewGuid(), Id, fieldDefinitionId, value));
        }
    }
}
