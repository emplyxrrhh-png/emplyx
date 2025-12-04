using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Employees;

public sealed class EmployeeUserField : Entity
{
    private EmployeeUserField() 
    {
        FieldDefinitionId = null!;
        Value = null!;
    }

    public EmployeeUserField(Guid id, Guid employeeId, string fieldDefinitionId, string value)
        : base(id)
    {
        EmployeeId = employeeId;
        FieldDefinitionId = fieldDefinitionId;
        Value = value;
    }

    public Guid EmployeeId { get; private set; }
    public string FieldDefinitionId { get; private set; }
    public string Value { get; private set; }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}
