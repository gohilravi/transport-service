using TransportService.Core.DTOs;

namespace TransportService.Core.Interfaces;

public interface ITransportService
{
    Task<CreateTransportResponse> CreateTransportAsync(CreateTransportRequest request);
    Task UpdateTransportStatusAsync(int id, string status);
    Task DeleteTransportAsync(int id, string elasticSearchId);
}