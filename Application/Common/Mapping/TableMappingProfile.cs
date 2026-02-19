using Application.Dto.Table;
using AutoMapper;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Common.Mapping
{
    public class TableMappingProfile : Profile
    {
        public TableMappingProfile()
        {
            // Map to TableResponse
            CreateMap<Table, TableResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.TableNumber))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
            
            // Map to TableDetailResponse
            CreateMap<Table, TableDetailResponse>()
                .ForMember(dest => dest.TableNumber, opt => opt.MapFrom(src => src.TableNumber))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.QRCode, opt => opt.MapFrom(src => src.QRCode))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));
        }
    }
}
