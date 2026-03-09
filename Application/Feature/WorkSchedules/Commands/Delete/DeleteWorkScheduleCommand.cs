using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace Application.Feature.WorkSchedules.Commands.Delete
{
    public record DeleteWorkScheduleCommand(Guid Id) : IRequest<Result<bool>>;
}
