using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateInfo
{
    public record UpdateInfoOrderCommand(Guid Id,Guid TableId,OrderType OrderType) : IRequest<Result<Guid>>;
}
