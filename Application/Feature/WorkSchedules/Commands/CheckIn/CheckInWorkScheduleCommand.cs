using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace Application.Feature.WorkSchedules.Commands.CheckIn
{
    public record CheckInWorkScheduleCommand(Guid Id) : IRequest<Result<bool>>;
}
