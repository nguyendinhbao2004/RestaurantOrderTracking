using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetAllOrderItem
{
    public record GetAllOrderItemsQuery(string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<OrderItemResponse>>
    {
    }
}
