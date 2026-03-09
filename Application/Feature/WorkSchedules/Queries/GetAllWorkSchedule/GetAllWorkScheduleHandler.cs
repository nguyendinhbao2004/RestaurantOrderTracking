using Application.Dto.WorkSchedule;
using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Feature.WorkSchedules.Queries.GetAllWorkSchedule
{
    public class GetAllWorkScheduleHandler : IRequestHandler<GetAllWorkScheduleQueries, PagedResult<WorkScheduleResponse>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IMapper _mapper;

        public GetAllWorkScheduleHandler(IWorkScheduleRepository workScheduleRepository, IMapper mapper)
        {
            _workScheduleRepository = workScheduleRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<WorkScheduleResponse>> Handle(GetAllWorkScheduleQueries request, CancellationToken cancellationToken)
        {
            var (schedules, totalCount) = await _workScheduleRepository.GetPagedWorkScheduleAsync(request.Keyword, request.PageIndex, request.PageSize);
            var scheduleResponses = _mapper.Map<List<WorkScheduleResponse>>(schedules);
            return new PagedResult<WorkScheduleResponse>(scheduleResponses, request.PageIndex, request.PageSize, totalCount, "Get work schedules list successful.");
        }
    }
}
