using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace Application.Feature.WorkSchedules.Commands.CheckOut
{
    public record CheckOutWorkScheduleCommand(Guid Id) : IRequest<Result<bool>>;
}
