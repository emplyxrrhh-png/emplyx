using Emplyx.Domain.Abstractions;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Emplyx.Infrastructure.Repositories;

internal abstract class RepositoryBase<TAggregate> : IRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{
    protected readonly EmplyxDbContext Context;
    protected readonly DbSet<TAggregate> DbSet;

    protected RepositoryBase(EmplyxDbContext context)
    {
        Context = context;
        DbSet = context.Set<TAggregate>();
    }

    public virtual async Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object?[] { id }, cancellationToken);
    }

    public virtual Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        return DbSet.AddAsync(aggregate, cancellationToken).AsTask();
    }

    public virtual void Update(TAggregate aggregate)
    {
        DbSet.Update(aggregate);
    }

    public virtual void Remove(TAggregate aggregate)
    {
        DbSet.Remove(aggregate);
    }
}
