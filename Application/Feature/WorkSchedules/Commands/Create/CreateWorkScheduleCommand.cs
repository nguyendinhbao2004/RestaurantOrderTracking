using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace Application.Feature.WorkSchedules.Commands.Create
{
    public record CreateWorkScheduleCommand(Guid AccountId, DateOnly WorkDate, TimeOnly StartTime, TimeOnly EndTime, string ShiftName, string? Note) : IRequest<Result<Guid>>;
}
