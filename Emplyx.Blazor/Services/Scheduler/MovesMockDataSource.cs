using Emplyx.Shared.Scheduler;

namespace Emplyx.Blazor.Services.Scheduler;

public sealed class MovesMockDataSource : IMovesDataSource
{
    private static readonly IReadOnlyList<DailyMoveListItem> Seed =
    [
        new("MV-0001", "Monica Rueda", "Terminal HQ", DateTimeOffset.UtcNow.AddMinutes(-45), "Entrada", "punch", "Confirmado", false),
        new("MV-0002", "Carlos Vidal", "Dock 3", DateTimeOffset.UtcNow.AddMinutes(-30), "Salida", "manual", "Pendiente", true),
        new("MV-0003", "Laura Estrada", "Lobby", DateTimeOffset.UtcNow.AddMinutes(-25), "Entrada", "calc", "Confirmado", false),
        new("MV-0004", "Daniel Ruiz", "Laboratorio", DateTimeOffset.UtcNow.AddMinutes(-20), "Break", "punch", "Confirmado", false),
        new("MV-0005", "Ana Morales", "Terminal HQ", DateTimeOffset.UtcNow.AddMinutes(-15), "Salida", "manual", "Rechazado", true)
    ];

    public Task<DailyMoveListResponse> GetAsync(DailyMoveListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<DailyMoveListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.EmployeeName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                item.Terminal.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Employee))
        {
            query = query.Where(item => item.EmployeeName.Contains(request.Employee, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Terminal))
        {
            query = query.Where(item => item.Terminal.Contains(request.Terminal, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            query = query.Where(item => item.Type.Equals(request.Type, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
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

        return Task.FromResult(new DailyMoveListResponse(items, total));
    }

    private static IEnumerable<DailyMoveListItem> ApplySort(IEnumerable<DailyMoveListItem> source, string field, bool desc) =>
        field.ToLowerInvariant() switch
        {
            "employeename" => desc ? source.OrderByDescending(x => x.EmployeeName) : source.OrderBy(x => x.EmployeeName),
            "terminal" => desc ? source.OrderByDescending(x => x.Terminal) : source.OrderBy(x => x.Terminal),
            "time" => desc ? source.OrderByDescending(x => x.Time) : source.OrderBy(x => x.Time),
            "type" => desc ? source.OrderByDescending(x => x.Type) : source.OrderBy(x => x.Type),
            "status" => desc ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            _ => desc ? source.OrderByDescending(x => x.Time) : source.OrderBy(x => x.Time)
        };
}
