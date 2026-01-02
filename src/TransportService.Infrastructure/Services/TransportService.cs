using AutoMapper;
using MassTransit;
using Newtonsoft.Json;
using TransportService.Core.Commands;
using TransportService.Core.DTOs;
using TransportService.Core.Entities;
using TransportService.Core.Interfaces;

namespace TransportService.Infrastructure.Services;

public class TransportService : ITransportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public TransportService(IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateTransportResponse> CreateTransportAsync(CreateTransportRequest request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var transport = _mapper.Map<Transport>(request);
            transport.Status = "Assigned";

            var createdTransport = await _unitOfWork.Transports.CreateAsync(transport);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Publish to Elasticsearch sync command
            var syncCommand = new SyncRecordInElasticSearch
            {
                ElasticSearchId = request.OfferId.ToString(),
                ObjectType = "Transport",
                Operation = "Create",
                Payload = JsonConvert.SerializeObject(new
                {
                    Id = createdTransport.Id,
                    CarrierId = createdTransport.CarrierId,
                    PurchaseId = createdTransport.PurchaseId,
                    PickupLocation = createdTransport.PickupLocation,
                    DeliveryLocation = createdTransport.DeliveryLocation,
                    ScheduleDate = createdTransport.ScheduleDate,
                    VehicleDetails = createdTransport.VehicleDetails,
                    Status = createdTransport.Status,
                    CreatedAt = createdTransport.CreatedAt,
                    LastModifiedAt = createdTransport.LastModifiedAt
                })
            };

            await _publishEndpoint.Publish(syncCommand);

            return new CreateTransportResponse { Id = createdTransport.Id };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpdateTransportStatusAsync(int id, string status)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var transport = await _unitOfWork.Transports.GetByIdAsync(id);
            if (transport == null)
            {
                throw new ArgumentException($"Transport with ID {id} not found.");
            }

            transport.Status = status;
            await _unitOfWork.Transports.UpdateAsync(transport);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Publish to Elasticsearch sync command
            var syncCommand = new SyncRecordInElasticSearch
            {
                ElasticSearchId = transport.OfferId.ToString(),
                ObjectType = "Transport",
                Operation = "Update",
                Payload = JsonConvert.SerializeObject(new
                {
                    Id = transport.Id,
                    CarrierId = transport.CarrierId,
                    PurchaseId = transport.PurchaseId,
                    PickupLocation = transport.PickupLocation,
                    DeliveryLocation = transport.DeliveryLocation,
                    ScheduleDate = transport.ScheduleDate,
                    VehicleDetails = transport.VehicleDetails,
                    Status = transport.Status,
                    CreatedAt = transport.CreatedAt,
                    LastModifiedAt = transport.LastModifiedAt
                })
            };

            await _publishEndpoint.Publish(syncCommand);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task DeleteTransportAsync(int id, string elasticSearchId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var transport = await _unitOfWork.Transports.GetByIdAsync(id);
            if (transport == null)
            {
                throw new ArgumentException($"Transport with ID {id} not found.");
            }

            // Cascade delete logic - delete related records first
            // Example: Delete related transport items, logs, etc.
            // await _unitOfWork.TransportItems.DeleteByTransportIdAsync(id);
            // await _unitOfWork.TransportLogs.DeleteByTransportIdAsync(id);

            await _unitOfWork.Transports.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Publish to Elasticsearch sync command
            var syncCommand = new SyncRecordInElasticSearch
            {
                ElasticSearchId = elasticSearchId,
                ObjectType = "Transport",
                Operation = "Delete",
                Payload = JsonConvert.SerializeObject(new { Id = id })
            };

            await _publishEndpoint.Publish(syncCommand);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}