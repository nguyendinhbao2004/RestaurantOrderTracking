using Application.Dto.Product;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Product.Queries.GetAllProduct
{
    public record GetAllProductQueries(string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<ProductResponse>>
    {
        
    }
}