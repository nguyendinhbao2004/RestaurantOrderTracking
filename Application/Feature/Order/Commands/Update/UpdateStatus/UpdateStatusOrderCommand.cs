using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateStatus
{
    public record UpdateStatusOrderCommand(Guid Id,OrderStatus NewStatus) : IRequest<Result<Guid>>;
}
