using Emplyx.Shared.Access;

namespace Emplyx.Blazor.Services.Access;

public sealed class AccessPeriodsMockDataSource : IAccessPeriodsDataSource
{
    private static readonly IReadOnlyList<AccessPeriodListItem> Seed =
    [
        new("AP-0001", "Oficina diurno", "Activo", "07:00-17:00 Lunes-Viernes", "L-V", "07:00", "17:00", "Todo el año", DateTimeOffset.UtcNow.AddDays(-1), false),
        new("AP-0002", "Planta nocturno", "Activo", "22:00-06:00 Turno Noche", "Todos", "22:00", "06:00", "Todo el año", DateTimeOffset.UtcNow.AddDays(-3), true),
        new("AP-0003", "Fin de semana", "Activo", "08:00-18:00 Sabado-Domingo", "S-D", "08:00", "18:00", "Todo el año", DateTimeOffset.UtcNow.AddDays(-5), false),
        new("AP-0004", "Auditoría mensual", "Suspendido", "Acceso primer día hábil", "Lunes", "06:00", "12:00", "Primer lunes", DateTimeOffset.UtcNow.AddDays(-20), true),
        new("AP-0005", "Mantenimiento", "Activo", "Slot 14:00-20:00", "Martes", "14:00", "20:00", "Trimestre 1", DateTimeOffset.UtcNow.AddDays(-10), false)
    ];

    public Task<AccessPeriodListResponse> GetAsync(AccessPeriodListRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<AccessPeriodListItem> query = Seed;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                (item.Description?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(item => item.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.WeekDay))
        {
            query = query.Where(item => item.WeekDayLabel.Contains(request.WeekDay, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Month))
        {
            query = query.Where(item => item.MonthLabel?.Contains(request.Month, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (request.IsSpecial.HasValue)
        {
            query = query.Where(item => item.IsSpecialPeriod == request.IsSpecial);
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

        return Task.FromResult(new AccessPeriodListResponse(items, total));
    }

    public Task<IReadOnlyList<AccessPeriodLookupItem>> SearchAsync(string? term, CancellationToken cancellationToken = default)
    {
        var query = Seed.Select(item => new AccessPeriodLookupItem(
            item.Id,
            item.Name,
            item.WeekDayLabel,
            $"{item.FromTime}-{item.ToTime}",
            item.Status));

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(item => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult<IReadOnlyList<AccessPeriodLookupItem>>(query.Take(20).ToList());
    }

    private static IEnumerable<AccessPeriodListItem> ApplySort(IEnumerable<AccessPeriodListItem> source, string field, bool descending) =>
        field.ToLowerInvariant() switch
        {
            "name" => descending ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name),
            "status" => descending ? source.OrderByDescending(x => x.Status) : source.OrderBy(x => x.Status),
            "weekdaylabel" => descending ? source.OrderByDescending(x => x.WeekDayLabel) : source.OrderBy(x => x.WeekDayLabel),
            "fromtime" => descending ? source.OrderByDescending(x => x.FromTime) : source.OrderBy(x => x.FromTime),
            "updatedat" => descending ? source.OrderByDescending(x => x.UpdatedAt) : source.OrderBy(x => x.UpdatedAt),
            _ => descending ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name)
        };
}
