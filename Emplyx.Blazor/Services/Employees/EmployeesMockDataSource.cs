using Emplyx.Shared.Employees;

namespace Emplyx.Blazor.Services.Employees;

/// <summary>
///     Fuente de datos temporal para Employees basada en el inventario 11.1.
///     Simula respuestas del adaptador `{ok,data,meta}` hasta que se conecte con legacy.
/// </summary>
public sealed class EmployeesMockDataSource : IEmployeesDataSource
{
    private static readonly IReadOnlyList<EmployeeListItem> Seed =
    [
        new(
            Id: "EMP-0001",
            Code: "0001",
            DisplayName: "Monica Rueda",
            Status: "Activo",
            PrimaryRole: "HR Admin",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-HQ", "HQ - Central"),
                new EmployeeGroupReference("GRP-OPS", "Operaciones Norte")
            },
            HasForgottenRight: false,
            Language: "es",
            Enabled: true),
        new(
            Id: "EMP-0002",
            Code: "0002",
            DisplayName: "Carlos Vidal",
            Status: "Activo",
            PrimaryRole: "Security Supervisor",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-ACC", "Access - Control"),
                new EmployeeGroupReference("GRP-HQ", "HQ - Central")
            },
            HasForgottenRight: true,
            Language: "es",
            Enabled: true),
        new(
            Id: "EMP-0003",
            Code: "0003",
            DisplayName: "Laura Estrada",
            Status: "Suspendido",
            PrimaryRole: "Planner",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-SCH", "Scheduler - Planner")
            },
            HasForgottenRight: false,
            Language: "en",
            Enabled: false),
        new(
            Id: "EMP-0004",
            Code: "0004",
            DisplayName: "Daniel Ruiz",
            Status: "Activo",
            PrimaryRole: "Tech Lead",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-BASE", "Base"),
                new EmployeeGroupReference("GRP-HQ", "HQ - Central")
            },
            HasForgottenRight: false,
            Language: "en",
            Enabled: true),
        new(
            Id: "EMP-0005",
            Code: "0005",
            DisplayName: "Ana Morales",
            Status: "Inactivo",
            PrimaryRole: "Security Admin",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-SEC", "Security - Admin")
            },
            HasForgottenRight: true,
            Language: "es",
            Enabled: false),
        new(
            Id: "EMP-0006",
            Code: "0006",
            DisplayName: "Mario Torres",
            Status: "Activo",
            PrimaryRole: "HR Senior",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-EMP", "Employees - Senior")
            },
            HasForgottenRight: false,
            Language: "es",
            Enabled: true),
        new(
            Id: "EMP-0007",
            Code: "0007",
            DisplayName: "Beatriz Vaca",
            Status: "Activo",
            PrimaryRole: "Security Analyst",
            Groups: new[]
            {
                new EmployeeGroupReference("GRP-ACC", "Access - Control")
            },
            HasForgottenRight: false,
            Language: "fr",
            Enabled: true)
    ];

    public Task<EmployeeListResponse> GetAsync(EmployeeListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<EmployeeListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.DisplayName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.Code.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.PrimaryRole.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Module))
        {
            query = query.Where(item => item.PrimaryRole.Contains(request.Module, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Group))
        {
            query = query.Where(item =>
                item.Groups.Any(group =>
                    group.Id.Equals(request.Group, StringComparison.OrdinalIgnoreCase) ||
                    group.Name.Contains(request.Group, StringComparison.OrdinalIgnoreCase)));
        }

        if (request.HasForgottenRight.HasValue)
        {
            query = query.Where(item => item.HasForgottenRight == request.HasForgottenRight);
        }

        if (!string.IsNullOrWhiteSpace(request.SortField))
        {
            query = ApplySort(query, request.SortField!, request.SortDescending);
        }

        var total = query.Count();
        var items = query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Task.FromResult(new EmployeeListResponse(items, total));
    }

    public Task<IReadOnlyList<EmployeeLookupItem>> SearchAsync(string? term, CancellationToken cancellationToken = default)
    {
        var query = Seed.Select(item => new EmployeeLookupItem(
            Id: item.Id,
            Code: item.Code,
            DisplayName: $"{item.DisplayName} · {item.PrimaryRole}",
            Module: item.PrimaryRole,
            Status: item.Status));

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(item =>
                item.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                item.Code.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult<IReadOnlyList<EmployeeLookupItem>>(query.Take(25).ToList());
    }

    private static IEnumerable<EmployeeListItem> ApplySort(IEnumerable<EmployeeListItem> source, string field, bool descending)
    {
        return field.ToLowerInvariant() switch
        {
            "code" => descending ? source.OrderByDescending(x => x.Code) : source.OrderBy(x => x.Code),
            "status" => descending ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            "primaryrole" => descending ? source.OrderByDescending(x => x.PrimaryRole) : source.OrderBy(x => x.PrimaryRole),
            _ => descending ? source.OrderByDescending(x => x.DisplayName) : source.OrderBy(x => x.DisplayName)
        };
    }
}
