using Emplyx.Shared.UI.DataGrid;

namespace Emplyx.Blazor.Services;

public sealed record SampleEmployeeRow(string Name, string Module, string Status, string Role);

public sealed class SampleEmployeesDataService
{
    private static readonly IReadOnlyList<SampleEmployeeRow> Seed =
    [
        new("Monica Rueda", "Employees", "Inventario listo", "HR Admin"),
        new("Carlos Vidal", "Access", "Parametros P0", "Security Supervisor"),
        new("Laura Estrada", "Scheduler", "Relevamiento", "Planner"),
        new("Daniel Ruiz", "Base", "Controls", "Tech Lead"),
        new("Ana Morales", "Security", "WIP", "Security Admin"),
        new("Mario Torres", "Employees", "Validaciones", "HR Senior"),
        new("Beatriz Vaca", "Access", "Contratos v0.1", "Security Analyst")
    ];

    public Task<DataGridResult<SampleEmployeeRow>> GetEmployeesAsync(DataGridCriteria criteria, CancellationToken cancellationToken = default)
    {
        IEnumerable<SampleEmployeeRow> query = Seed;

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(criteria.Search, StringComparison.OrdinalIgnoreCase) ||
                item.Module.Contains(criteria.Search, StringComparison.OrdinalIgnoreCase) ||
                item.Status.Contains(criteria.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.SortField))
        {
            query = ApplySort(query, criteria.SortField!, criteria.SortDescending);
        }

        var total = query.Count();
        var skip = criteria.PageIndex * criteria.PageSize;
        var items = query.Skip(skip).Take(criteria.PageSize).ToList();

        return Task.FromResult(new DataGridResult<SampleEmployeeRow>(items, total));
    }

    private static IEnumerable<SampleEmployeeRow> ApplySort(IEnumerable<SampleEmployeeRow> source, string field, bool descending) =>
        field switch
        {
            nameof(SampleEmployeeRow.Name) => descending ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name),
            nameof(SampleEmployeeRow.Module) => descending ? source.OrderByDescending(x => x.Module) : source.OrderBy(x => x.Module),
            nameof(SampleEmployeeRow.Status) => descending ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            nameof(SampleEmployeeRow.Role) => descending ? source.OrderByDescending(x => x.Role) : source.OrderBy(x => x.Role),
            _ => source
        };
}
