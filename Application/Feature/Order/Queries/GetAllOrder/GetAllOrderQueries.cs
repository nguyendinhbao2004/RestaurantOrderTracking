using MediatR;
using RestaurantOrderTracking.Application.Dto.Order;
using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Queries.GetAllOrder
{
    public record GetAllOrderQueries(string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<OrderResponse>>
    {
    }
}
