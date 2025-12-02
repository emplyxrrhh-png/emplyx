using Emplyx.Shared.Employees;

namespace Emplyx.Blazor.Services.Employees;

public interface IEmployeesDataSource
{
    Task<EmployeeListResponse> GetAsync(EmployeeListRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmployeeLookupItem>> SearchAsync(string? term, CancellationToken cancellationToken = default);

    Task CreateAsync(EmployeeEditModel model, CancellationToken cancellationToken = default);
}
