using Application.Dto.Table;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Queries.GetByAreaId
{
    public record GetTablesByAreaIdQueries(Guid AreaId) : IRequest<Result<IEnumerable<TableDetailResponse>>>
    {
        
    }
}
