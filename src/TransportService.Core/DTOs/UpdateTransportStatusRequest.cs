namespace TransportService.Core.DTOs;

public class UpdateTransportStatusRequest
{
    public string ElasticSearchId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}