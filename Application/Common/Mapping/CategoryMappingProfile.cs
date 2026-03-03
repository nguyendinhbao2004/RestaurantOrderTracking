using AutoMapper;
using RestaurantOrderTracking.Application.Dto.Category;
using RestaurantOrderTracking.Domain.Entities;

namespace RestaurantOrderTracking.Application.Common.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryResponse>();
        }
    }
}
