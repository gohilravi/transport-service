using TransportService.Core.DTOs;

namespace TransportService.Core.Interfaces;

public interface ITransportService
{
    Task<CreateTransportResponse> CreateTransportAsync(CreateTransportRequest request, string elasticSearchId);
    Task UpdateTransportStatusAsync(int id, string status, string elasticSearchId);
    Task DeleteTransportAsync(int id, string elasticSearchId);
}