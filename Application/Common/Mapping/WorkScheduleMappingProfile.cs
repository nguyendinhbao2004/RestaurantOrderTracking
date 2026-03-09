using Application.Dto.WorkSchedule;
using AutoMapper;
using RestaurantOrderTracking.Domain.Entities;

namespace Application.Common.Mapping
{
    public class WorkScheduleMappingProfile : Profile
    {
        public WorkScheduleMappingProfile()
        {
            CreateMap<WorkSchedule, WorkScheduleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.WorkDate, opt => opt.MapFrom(src => src.WorkDate))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.ActualCheckIn, opt => opt.MapFrom(src => src.ActualCheckIn))
                .ForMember(dest => dest.ActualCheckOut, opt => opt.MapFrom(src => src.ActualCheckOut))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));
        }
    }
}
