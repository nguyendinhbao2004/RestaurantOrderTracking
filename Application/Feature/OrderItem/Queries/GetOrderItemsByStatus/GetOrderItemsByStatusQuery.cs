using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetOrderItemsByStatus
{
    public record GetOrderItemsByStatusQuery(int Status) : IRequest<List<OrderItemDetailResponse>>;
}
