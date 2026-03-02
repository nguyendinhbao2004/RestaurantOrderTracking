using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Create
{
    public record CreateOrderCommand(Guid? TableId,Guid? CustomerId,Guid AccountId,OrderType OrderType) : IRequest<Result<Guid>>;
}
