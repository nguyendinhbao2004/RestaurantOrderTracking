using MediatR;
using RestaurantOrderTracking.Application.Dto.Order;
using System;

namespace RestaurantOrderTracking.Application.Feature.Order.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDetailResponse>;
}
