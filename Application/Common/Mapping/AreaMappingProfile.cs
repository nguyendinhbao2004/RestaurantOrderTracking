using AutoMapper;
using RestaurantOrderTracking.Application.Dto.Area;
using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Application.Common.Mapping
{
    public class AreaMappingProfile : Profile
    {
        public AreaMappingProfile()
        {
            CreateMap<Area, AreaResponse>();
            
            CreateMap<Domain.Entities.Waiter, AreaWaiterResponse>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName));
        }
    }
}
