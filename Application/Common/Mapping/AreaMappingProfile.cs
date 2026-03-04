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
        }
    }
}
