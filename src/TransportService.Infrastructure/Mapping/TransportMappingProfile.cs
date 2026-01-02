using AutoMapper;
using TransportService.Core.DTOs;
using TransportService.Core.Entities;

namespace TransportService.Infrastructure.Mapping;

public class TransportMappingProfile : Profile
{
    public TransportMappingProfile()
    {
        CreateMap<CreateTransportRequest, Transport>()
            .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => src.SellerZipCode))
            .ForMember(dest => dest.DeliveryLocation, opt => opt.MapFrom(src => src.BuyerZipCode))
            .ForMember(dest => dest.ScheduleDate, opt => opt.MapFrom(src => src.ScheduleWindow.StartDate))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleDetails, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());
    }
}