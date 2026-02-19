using Application.Dto.Table;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Tables.Queries.GetById
{
    public record GetTableByIdQueries(Guid Id) : IRequest<Result<TableDetailResponse>>
    {
        
    }
}