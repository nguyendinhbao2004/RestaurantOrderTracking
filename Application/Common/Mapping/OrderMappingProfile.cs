using AutoMapper;
using RestaurantOrderTracking.Application.Dto.Order;
using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Common.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile() {
            CreateMap<Order,OrderResponse>()
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.Table.TableNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
