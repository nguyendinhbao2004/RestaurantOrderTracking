using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetOrderItemsByAccount
{
    public record GetOrderItemsByAccountQuery(Guid AccountId) : IRequest<Result<IEnumerable<OrderItemByAccountResponse>>>
    {
    }
}
