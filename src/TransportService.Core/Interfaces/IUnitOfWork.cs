namespace TransportService.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITransportRepository Transports { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}