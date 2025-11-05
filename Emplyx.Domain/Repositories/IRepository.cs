using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Repositories;

public interface IRepository<TAggregate> where TAggregate : class, IAggregateRoot
{
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    void Update(TAggregate aggregate);

    void Remove(TAggregate aggregate);
}
