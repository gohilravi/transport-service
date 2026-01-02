using MassTransit;

namespace TransportService.Core.Commands;

[EntityName("SyncRecordInElasticSearch")]
public class SyncRecordInElasticSearch
{
    public string ElasticSearchId { get; set; } = string.Empty;
    public string ObjectType { get; set; } = "Transport";
    public string Operation { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}