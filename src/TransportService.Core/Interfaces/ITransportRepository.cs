using TransportService.Core.Entities;

namespace TransportService.Core.Interfaces;

public interface ITransportRepository
{
    Task<Transport?> GetByIdAsync(int id);
    Task<Transport> CreateAsync(Transport transport);
    Task<Transport> UpdateAsync(Transport transport);
    Task DeleteAsync(int id);
}