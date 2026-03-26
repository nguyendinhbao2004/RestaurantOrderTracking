using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetConfirmedOrderItems
{
    public record GetConfirmedOrderItemsQuery(int PageIndex, int PageSize) : IRequest<PagedResult<ConfirmedOrderItemResponse>>;
}