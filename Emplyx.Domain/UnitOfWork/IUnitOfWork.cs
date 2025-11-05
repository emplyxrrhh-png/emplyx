namespace Emplyx.Domain.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
