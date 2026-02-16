using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Queries.GetAllTable
{
    public record GetAllTableQueries(string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<TableResponse>>
    {
        
    }
}