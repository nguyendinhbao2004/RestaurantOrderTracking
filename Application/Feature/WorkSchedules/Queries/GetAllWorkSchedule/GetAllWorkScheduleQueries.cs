using Application.Dto.WorkSchedule;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.WorkSchedules.Queries.GetAllWorkSchedule
{
    public record GetAllWorkScheduleQueries(string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<WorkScheduleResponse>>;
}
