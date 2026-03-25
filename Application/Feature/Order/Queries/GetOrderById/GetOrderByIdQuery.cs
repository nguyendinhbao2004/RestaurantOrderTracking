using MediatR;
using RestaurantOrderTracking.Application.Dto.Order;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Order.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDetailResponse>>;
}
