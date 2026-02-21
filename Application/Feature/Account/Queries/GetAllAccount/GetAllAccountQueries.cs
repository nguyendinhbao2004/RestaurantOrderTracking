using Application.Dto.Account;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Account.Queries.GetAllAccount
{
    public record GetAllAccountQueries (string? Keyword, int PageIndex, int PageSize) : IRequest<PagedResult<AccountResponse>>
    {
        
    }
}