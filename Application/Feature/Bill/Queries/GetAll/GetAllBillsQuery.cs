using MediatR;
using RestaurantOrderTracking.Application.Dto.Bill;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Bill.Queries.GetAll
{
    public record GetAllBillsQuery(string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<BillResponse>>;
}
